using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using SWP.Dao;

namespace SWP.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            using (var context = new SWPContext())
            {
                var totalOrders = context.Orders.Count();

                var orderList = context.Orders
                    .OrderBy(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View(orderList);
            }
        }

        public IActionResult ViewDetail(int id)
        {
            using (var context = new SWPContext())
            {
                var orderInfo = context.Orders
                    .Where(o => o.OrderId == id)
                    .FirstOrDefault();

                if (orderInfo == null)
                {
                    return NotFound();
                }

                var orderDetails = context.Orderdetails
                    .Where(od => od.OrderId == id)
                    .ToList();

                decimal totalAmount = orderDetails.Sum(od => (od.Quantity ?? 0) * (od.Price ?? 0));

                var productDetailIds = orderDetails.Select(od => od.DetailId).ToList();

                var productDetails = context.ProductDetails
                    .Include(pd => pd.Product)
                        .ThenInclude(p => p.ProductImages)
                    .Include(pd => pd.Color)
                    .Include(pd => pd.Size)
                    .Where(pd => productDetailIds.Contains(pd.DetailId))
                    .ToList();

                var viewModel = new OrderDetailViewModel
                {
                    OrderId = orderInfo.OrderId,
                    CustomerName = orderInfo.CustomerName,
                    ShipAddress = orderInfo.ShipAddress,
                    OrderDate = orderInfo.OrderDate,
                    PhoneNumber = orderInfo.PhoneNumber,
                    Status = orderInfo.Status,
                    TotalAmount = totalAmount,
                    OrderProductDetails = orderDetails.Select(od => new OrderProductDetailViewModel
                    {
                        ProductName = productDetails.FirstOrDefault(pd => pd.DetailId == od.DetailId)?.Product?.ProductName,
                        Price = od.Price ?? 0,
                        Quantity = od.Quantity ?? 0,
                        ColorName = productDetails.FirstOrDefault(pd => pd.DetailId == od.DetailId)?.Color?.ColorName,
                        SizeName = productDetails.FirstOrDefault(pd => pd.DetailId == od.DetailId)?.Size?.SizeName,
                        ProductImages = productDetails.FirstOrDefault(pd => pd.DetailId == od.DetailId)?.Product?.ProductImages.Select(pi => pi.Path).ToList()
                    }).ToList()
                };

                return View(viewModel); // Chuyển đối tượng viewModel sang View
            }
        }
    }
}
