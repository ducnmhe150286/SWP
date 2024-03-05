using Microsoft.AspNetCore.Mvc;
using SWP.Dao;
using SWP.Models;

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
                if(selectedItems.Count == 0)
                {
                    return RedirectToAction("GetCartItem", "Cart", new { message = 1 });
                }
                var checkout = checkOutDao.getAddItem(cusId.UserId , selectedItems);
                var infor = checkOutDao.getInformation(cusId.UserId);
                ViewData["infor"] = infor;
                ViewData["listItem"] = checkout;
                return View();
            }
                //get detail
                // formdata[]                
            return RedirectToAction("Index", "Auth");
        }
        [HttpPost]
        public IActionResult AddDB(string customerName,string phoneNumber,string address,List<int> listItem1)
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");
            var cusId = userDao.GetUserByEmail(customer);
            var check = checkOutDao.adddb(cusId.UserId, customerName, phoneNumber, address, listItem1);

            return RedirectToAction("Index", "Home");
        }
    }
}
