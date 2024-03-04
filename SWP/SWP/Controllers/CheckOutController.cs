using Microsoft.AspNetCore.Mvc;
using SWP.Dao;

namespace SWP.Controllers
{
    public class CheckOutController : Controller
    {
        public UsersDao userDao;
        public CheckOutDao checkOutDao;

        public CheckOutController()
        {
            userDao= new UsersDao();
            checkOutDao = new CheckOutDao();
        }
        [HttpPost]
        public IActionResult Index(List<int> selectedItems)
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");
            var cusId = userDao.GetUserByEmail(customer);
            if (cusId is not null && cusId.RoleId == 2)
            {
                var checkout = checkOutDao.getAddItem(cusId.UserId , selectedItems);
                ViewData["listItem"] = checkout;
                return View();
            }
                //get detail
                // formdata[]                
            return RedirectToAction("Index", "Auth");
        }
    }
}
