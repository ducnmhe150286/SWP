using Microsoft.AspNetCore.Mvc;
using SWP.Dao;

namespace SWP.Controllers
{
    public class OrderHistoryController : Controller
    {
        public UsersDao userDao;
        public OrderHistoryDao historyDao;
        public OrderHistoryController()
        {
            userDao = new UsersDao();
            historyDao = new OrderHistoryDao();
        }
        public IActionResult Index()
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");

            var cusId = userDao.GetUserByEmail(customer);
            if (cusId is not null && cusId.RoleId == 2)
            {
                var getOrder = historyDao.getAllOrder(cusId.UserId);
                ViewData["listItem"] = getOrder;
                return View();
            }
                return RedirectToAction("Index","Auth");
        }
    }
}
