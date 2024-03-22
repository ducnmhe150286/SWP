using Microsoft.AspNetCore.Mvc;
using SWP.Dao;
using SWP.Models;

namespace SWP.Controllers
{
    public class CartController : Controller
    {
        public CartDao cartDao;
        public UsersDao userDao;
        public CartController()
        {
            cartDao = new CartDao();
            userDao = new UsersDao();
        }
        public IActionResult Index(int productId, int quantity, int size, int color)
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");

            var cusId = userDao.GetUserByEmail(customer);
            if (cusId is not null && cusId.RoleId == 2)
            {
                var cartItem = cartDao.addToCart(productId, quantity, size, color, cusId.UserId);

                var listItem = cartDao.GetAddItem(cusId.UserId);
                ViewData["listItem"] = listItem;
                return View();
                /*var check = cartDao.CheckQuantity(productId, quantity, size, color);
                if(check == 0)
                {
                    var cartItem = cartDao.addToCart(productId, quantity, size, color, cusId.UserId);

                    var listItem = cartDao.GetAddItem(cusId.UserId);
                    ViewData["listItem"] = listItem;
                    return View();
                }
                TempData["productId"] = productId;
                TempData["error_quantity"] = $"Số lượng sản phẩm còn: {check} không đủ để mua! Vui lòng chọn sản phẩm khác";
                return RedirectToAction("Index", "ProductDetail");*/
                
            }
            return RedirectToAction("Index", "Auth");
        }

        public IActionResult GetCartItem(int? message)
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");
            if (message == 1) {
                ViewData["message"] = "Vui long chon san pham thanh toán";
            }
            var cusId = userDao.GetUserByEmail(customer);
            if (cusId is not null && cusId.RoleId == 2)
            {
                var listItem = cartDao.GetAddItem(cusId.UserId);
                ViewData["listItem"] = listItem;
                return View("Index");
            }
            return RedirectToAction("Index", "Auth");
        }
        public IActionResult UpdateCartItem(int productId, int type)
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");
            var cusId = userDao.GetUserByEmail(customer);
            var update = cartDao.updateCartItem(productId, type, cusId.UserId);
            return RedirectToAction("Index", new { productId = productId });
        }
    }
}
