using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using SWP.Dao;

namespace SWP.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index(int? searchStatus, int page = 1, int pageSize = 5)
        {
            using (var context = new SWPContext())
            {
                var query = context.Orders.AsQueryable();

                // Lọc theo trạng thái nếu được chỉ định
                if (searchStatus.HasValue)
                {
                    query = query.Where(o => o.Status == searchStatus);
                }

                var totalOrders = query.Count();

                var orderList = query
                    .OrderBy(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                // Truyền tham số searchStatus vào view để giữ trạng thái tìm kiếm khi phân trang
                ViewBag.SearchStatus = searchStatus;

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
        public ActionResult BackToIndex()
        {
            // Thực hiện các xử lý cần thiết (nếu có)

            // Điều hướng người dùng về trang Index
            return RedirectToAction("Index"); // Thay "Home" bằng tên controller chứa action Index nếu cần
        }
        [HttpPost]
        public ActionResult UpdateStatus(int orderId, int status)
        {
            using (var context = new SWPContext())
            {
                // Lấy order từ cơ sở dữ liệu
                var order = context.Orders.Find(orderId);
                if (order != null)
                {
                    // Chuyển đổi kiểu dữ liệu 'int' sang 'byte?'
                    byte? byteStatus = (byte?)status;
                    // Cập nhật trạng thái
                    order.Status = byteStatus;
                    // Lưu thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();
                }
                return Json(new { success = true });
            }
        }
    }
}
