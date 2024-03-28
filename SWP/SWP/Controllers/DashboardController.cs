using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Dao;
using SWP.Models;
using SWP.Utilities;
using System;
using System.Globalization;
using System.Linq;

namespace SWP.Controllers
{

    public class DashboardController : Controller
    {
        public UsersDao userDao;
        public DashboardController()
        {
            userDao = new UsersDao();
        }
        public User GetUserByEmail(string email)
        {
            try
            {
                using (var connection = new SWPContext())
                {
                    var user = connection.Users.Where(x => x.Email == email).FirstOrDefault();
                    return user;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IActionResult Index(int page = 1, int selectedYear = 0)
		{
            var customer = HttpContext.Session.GetString("USER_EMAIL");
            var cusId = userDao.GetUserByEmail(customer);

            if (cusId is not null && cusId.RoleId == 1)
			{

				using (var context = new SWPContext())
				{

					int pageSize = 6;
					// Lấy ngày đầu tiên của năm hiện tại
					DateTime firstDayOfYear = new DateTime(selectedYear != 0 ? selectedYear : DateTime.Now.Year, 1, 1);

					// Tạo mảng để lưu trữ dữ liệu doanh thu của 12 tháng
					int[] monthlyRevenues = new int[12];

					for (int month = 1; month <= 12; month++)
					{
						// Lấy ngày đầu tiên của tháng
						DateTime firstDayOfMonth = new DateTime(selectedYear != 0 ? selectedYear : DateTime.Now.Year, month, 1);
						// Lấy ngày cuối cùng của tháng
						DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

						// Lấy doanh thu cho tháng hiện tại
						decimal totalRevenueByMonth = context.Orders
												.Where(o => o.Status == 3 &&
															o.ShipDate.HasValue &&
															o.ShipDate.Value >= firstDayOfMonth &&
															o.ShipDate.Value <= lastDayOfMonth)
												.SelectMany(o => o.Orderdetails)
												.Sum(od => (od.Quantity ?? 0) * (od.Price ?? 0));

						// Đổi từ decimal sang int
						int integerRevenue = (int)Math.Round(totalRevenueByMonth);

						// Lưu doanh thu vào mảng
						monthlyRevenues[month - 1] = integerRevenue == 0 ? 0 : integerRevenue;
					}



					DateTime currentDate = DateTime.Now.Date;

					decimal totalRevenueOneDay = context.Orders
											.Where(o => o.Status == 3 &&
														o.ShipDate.HasValue && // Kiểm tra OrderDate có giá trị không
														o.ShipDate.Value.Date == currentDate)
											.SelectMany(o => o.Orderdetails)
											.Sum(od => (od.Quantity ?? 0) * (od.Price ?? 0));




					int newOrdersCount = context.Orders.Count(o => o.Status == 0);
					int CancelOrdersCount = context.Orders.Count(o => o.Status == 5);

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

			})
			.GroupBy(tp => tp.ProductName)
			.Select(g => new
			{
				ProductName = g.Key,
				TotalQuantity = g.Sum(tp => tp.TotalQuantity),
				TotalRevenue = g.Sum(tp => tp.TotalRevenue),
			})
			.Take(5)
			.ToList();



					var allOrders = context.Orders.OrderByDescending(g => g.OrderDate)
	.ToList();
					var paginatedList = PaginatedList<Order>.Create(allOrders.AsQueryable(), page, pageSize);

					// Gửi tổng tiền thu được, số lượng đơn hàng mới, tổng số người dùng, và top 5 sản phẩm đến view
					ViewBag.totalRevenueOneDay = totalRevenueOneDay;
					ViewBag.AllOrders = paginatedList;
					ViewBag.NewOrders = newOrdersCount;
					ViewBag.TotalUsers = totalUsersCount;
					ViewBag.CancelOrdersCount = CancelOrdersCount;
					ViewBag.TopSellingProducts = topSellingProductsInfo;
					ViewBag.Revenues = monthlyRevenues;
					ViewBag.SelectedYear = selectedYear;
				}
                return View();

            }
            return RedirectToAction("Index", "Auth");

		}
	}
}
