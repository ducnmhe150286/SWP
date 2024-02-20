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
        public IActionResult Index(int productId, int quantity)
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");
            
            var cusId = userDao.GetUserByEmail(customer);
            if (cusId is not null && cusId.RoleId == 2 )
            {
                var cartItem = cartDao.addToCart(productId, quantity, cusId.UserId);
                var listItem = cartDao.GetAddItem(cusId.UserId);
                ViewData["listItem"] = listItem;
                return View();              
            }
            return RedirectToAction("Index", "Auth");
        }

        public IActionResult GetCartItem()
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");

            var cusId = userDao.GetUserByEmail(customer);
            if (cusId is not null && cusId.RoleId == 2)
            {
                var listItem = cartDao.GetAddItem(cusId.UserId);
                ViewData["listItem"] = listItem;
                return View("Index");
            }
            return RedirectToAction("Index", "Auth");
        }
        public IActionResult UpdateCartItem(int productId,int type)
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");
            var cusId = userDao.GetUserByEmail(customer);
                var update = cartDao.updateCartItem(productId, type, cusId.UserId);
            return RedirectToAction("Index");
        }
    }
}
