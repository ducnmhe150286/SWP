using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using SWP.Utilities;
using System.Linq;

namespace SWP.Controllers
{
	public class DashboardController : Controller
	{
		public IActionResult Index(int page = 1)
		{
			using (var context = new SWPContext())
			{
				int pageSize = 6;

				decimal totalRevenue = context.Orders
					.Where(o => o.Status == 3)
					.SelectMany(o => o.Orderdetails)
					.Sum(od => (od.Quantity ?? 0) * (od.Price ?? 0));

				int newOrdersCount = context.Orders.Count(o => o.Status == 0);
				int totalUsersCount = context.Users.Count(u => u.RoleId != 1);

				var topSellingProducts = context.Orders
					.Where(o => o.Status == 3)
					.SelectMany(o => o.Orderdetails)
					.GroupBy(od => od.DetailId)
					.Select(g => new
					{
						ProductId = g.Key,
						TotalQuantity = g.Sum(od => od.Quantity),
						TotalRevenue = g.Sum(od => (od.Quantity ?? 0) * (od.Price ?? 0))
					})
					.OrderByDescending(g => g.TotalQuantity)
					.ToList();

				var productNames = context.ProductDetails
					.Join(context.Products, pd => pd.ProductId, p => p.ProductId, (pd, p) => new { pd.DetailId, p.ProductName })
					.ToDictionary(pd => pd.DetailId, pd => pd.ProductName);

				var topSellingProductsInfo = topSellingProducts
					.Select(tp => new
					{
						ProductId = tp.ProductId,
						ProductName = productNames.ContainsKey(tp.ProductId) ? productNames[tp.ProductId] : "Unknown Product",
						TotalQuantity = tp.TotalQuantity,
						TotalRevenue = tp.TotalRevenue,
						ProductImages = context.ProductImages.Where(pi => pi.ProductId == tp.ProductId).ToList()
					})
					.GroupBy(tp => tp.ProductName)
					.Select(g => new
					{
						ProductName = g.Key,
						TotalQuantity = g.Sum(tp => tp.TotalQuantity),
						TotalRevenue = g.Sum(tp => tp.TotalRevenue),
						ProductImages = g.SelectMany(tp => tp.ProductImages).ToList()
					})
					.Take(5)
					.ToList();

				var allOrders = context.Orders.OrderByDescending(g => g.OrderDate)
.ToList();
				var paginatedList = PaginatedList<Order>.Create(allOrders.AsQueryable(), page, pageSize);

				// Gửi tổng tiền thu được, số lượng đơn hàng mới, tổng số người dùng, và top 5 sản phẩm đến view
				ViewBag.TotalRevenue = totalRevenue;
				ViewBag.AllOrders = paginatedList;
				ViewBag.NewOrders = newOrdersCount;
				ViewBag.TotalUsers = totalUsersCount;
				ViewBag.TopSellingProducts = topSellingProductsInfo;
			}

			return View();
		}
	}
}
