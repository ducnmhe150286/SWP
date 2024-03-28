using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using SWP.Dao;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.CodeAnalysis;


namespace SWP.Controllers
{
    public class OrderController : Controller
    {

        public IActionResult Index(int? searchStatus, int page = 1, int pageSize = 5)
        {
            // Lấy userId từ session
            var userId = HttpContext.Session.GetInt32("USER_ID");


            // Kiểm tra userId có null không
            if (userId == null)
            {
                // Nếu userId null, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }
            else
            {
                using (var context = new SWPContext())
                {
                    // Lấy RoleId của User dựa trên userId, chẳng hạn từ CSDL hoặc bất kỳ nguồn dữ liệu nào khác
                    var user = context.Users.Find(userId);

                    // Kiểm tra nếu RoleId của User là 2
                    if (user != null && user.RoleId == 2)
                    {
                        // Thực hiện các hành động nếu RoleId của User là 2
                        return RedirectToAction("Login", "Account");
                    }
                }
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
                    CancelReason = orderInfo.CancelReason,
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
                var order = context.Orders.Find(orderId);
                if (order != null)
                {
                    byte? byteStatus = (byte?)status;
                    if (IsValidTransition(order.Status, byteStatus))
                    {
                        order.Status = byteStatus;

                        // Nếu trạng thái là 'Giao hàng thành công', cập nhật trường ShipDate với ngày hiện tại
                        if (status == 3)
                        {
                            order.ShipDate = DateTime.Now;
                        }

                        context.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                    {
                        // Trả về một thông báo lỗi nếu không thể chuyển từ trạng thái hiện tại sang trạng thái mới
                        return Json(new { success = false, errorMessage = "Không thể cập nhật trạng thái đến trạng thái này." });
                    }
                }
                return Json(new { success = false, errorMessage = "Không tìm thấy đơn hàng." });
            }
        }



        private bool IsValidTransition(byte? currentStatus, byte? newStatus)
        {
            if (currentStatus == null) // Trường hợp đặc biệt: trạng thái hiện tại là null
            {
                return newStatus == 0; // Chỉ cho phép bắt đầu từ trạng thái "Chờ xác nhận"
            }

            // Các quy tắc chuyển trạng thái ở đây
            switch (currentStatus)
            {
                case 0: // Trạng thái hiện tại là "Chờ xác nhận"
                    return newStatus == 1; // Chỉ cho phép chuyển sang trạng thái "Xác nhận đơn hàng"
                case 1: // Trạng thái hiện tại là "Xác nhận đơn hàng"
                    return newStatus == 2; // Chỉ cho phép chuyển sang trạng thái "Đang vận chuyển"
                case 2: // Trạng thái hiện tại là "Đang vận chuyển"
                    return newStatus == 3 || newStatus == 4; // Chỉ cho phép chuyển sang trạng thái "Giao hàng thành công" hoặc "Giao hàng không thành công"
                case 3: // Trạng thái hiện tại là "Giao hàng thành công"
                case 4: // Trạng thái hiện tại là "Giao hàng không thành công"
                    return false; // Không cho phép chuyển sang trạng thái mới từ đây
                default:
                    return false; // Trạng thái không hợp lệ
            }
        }



        public IActionResult CustomerView(byte? status = null)
        {
            // Lấy userId từ session
            var userId = HttpContext.Session.GetInt32("USER_ID");

            // Kiểm tra userId có null không
            if (userId == null)
            {
                // Nếu userId null, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            using (var context = new SWPContext())
            {
                IQueryable<Order> ordersQuery = context.Orders.Where(o => o.UserId == userId);

                // Thêm điều kiện lọc theo status nếu status được chỉ định
                if (status != null)
                {
                    ordersQuery = ordersQuery.Where(o => o.Status == status);
                }

                var orders = ordersQuery.ToList();
                var orderProductDetails = new List<OrderProductDetailViewModel>();

                foreach (var order in orders)
                {
                    var orderDetails = context.Orderdetails.Where(od => od.OrderId == order.OrderId).ToList();

                    foreach (var orderDetail in orderDetails)
                    {
                        var productDetail = context.ProductDetails
                            .Include(pd => pd.Product)
                                .ThenInclude(p => p.ProductImages)
                            .Include(pd => pd.Color)
                            .Include(pd => pd.Size)
                            .FirstOrDefault(pd => pd.DetailId == orderDetail.DetailId);

                        if (productDetail != null)
                        {
                            orderProductDetails.Add(new OrderProductDetailViewModel
                            {
                                ProductName = productDetail.Product.ProductName,
                                Price = orderDetail.Price ?? 0,
                                Quantity = orderDetail.Quantity ?? 0,
                                ColorName = productDetail.Color?.ColorName,
                                SizeName = productDetail.Size?.SizeName,
                                ProductImages = productDetail.Product.ProductImages.Select(pi => pi.Path).ToList(),
                                OrderId = order.OrderId
                            });
                        }
                    }
                }

                // Truyền danh sách sản phẩm đến view và thêm status vào ViewBag để sử dụng trong view
                ViewBag.Status = status;
                return View(orderProductDetails);
            }
        }


        public IActionResult ProductDetail(int orderId)
        {
            // Lấy userId từ session
            var userId = HttpContext.Session.GetInt32("USER_ID");

            // Kiểm tra userId có null không
            if (userId == null)
            {
                // Nếu userId null, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }
            using (var context = new SWPContext())
            {
                // Kiểm tra orderId có hợp lệ hay không
                if (orderId <= 0)
                {
                    // Nếu không hợp lệ, chuyển hướng đến trang thông báo lỗi
                    return View("OrderNotFound");
                }

                // Lấy thông tin đơn hàng chứa sản phẩm đang được chọn
                var order = context.Orders.FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);

                if (order == null)
                {
                    // Nếu không tìm thấy đơn hàng, chuyển hướng đến trang thông báo lỗi
                    return View("OrderNotFound");
                }

                var orderDetailViewModel = new OrderDetailViewModel
                {
                    OrderId = order.OrderId,
                    Status = order.Status.HasValue ? order.Status.Value : default(byte),
                    // Gán thông tin khách hàng vào ViewModel
                    CustomerName = order.CustomerName,
                    PhoneNumber = order.PhoneNumber,
                    ShipAddress = order.ShipAddress,
                    OrderDate = order.OrderDate // Giả sử bạn có thuộc tính OrderDate trong model Order
                };

                return View("OrderStatus", orderDetailViewModel);
            }
        }
        [HttpPost]
        public IActionResult CancelOrder(int orderId, string cancelReason)
        {
            using (var context = new SWPContext())
            {
                var order = context.Orders.FirstOrDefault(o => o.OrderId == orderId);

                if (order == null)
                {
                    return NotFound(); // Trả về mã lỗi nếu không tìm thấy đơn hàng
                }

                if (order.Status != 0 && order.Status != 1)
                {
                    return BadRequest("Không thể hủy đơn hàng với trạng thái hiện tại."); // Trả về mã lỗi nếu không thể hủy đơn hàng với trạng thái hiện tại
                }

                order.Status = 5; // Cập nhật trạng thái đơn hàng thành "Đã hủy"
                order.CancelReason = cancelReason; // Lưu lý do hủy vào trường CancelReason

                context.SaveChanges(); // Lưu thay đổi vào database

                // Thêm thông báo trạng thái thành công vào TempData để hiển thị trong view
                TempData["CancelSuccess"] = true;

                return Ok(); // Trả về mã thành công
            }
        }
    }
}
