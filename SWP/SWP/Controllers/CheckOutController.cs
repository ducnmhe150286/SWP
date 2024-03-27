using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Dao;
using SWP.Models;
using System.IO;

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
        public IActionResult Index()
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");
            var se = HttpContext.Session.GetString("SelectedItems1");
            List<int> selectedItems1 = new List<int>();
      
            String[] item = se.Split("/");

            int[] numbers = Array.ConvertAll(item, int.Parse);
            foreach (var item1 in numbers)
            {
                selectedItems1.Add(item1);
            }
            var cusId = userDao.GetUserByEmail(customer);
            if (cusId is not null && cusId.RoleId == 2)
            {
                if (selectedItems1.Count == 0)
                {
                    return RedirectToAction("GetCartItem", "Cart", new { message = 1 });
                }
                var checkout = checkOutDao.getAddItem(cusId.UserId, selectedItems1);
                var infor = checkOutDao.getInformation(cusId.UserId);
                ViewData["infor"] = infor;
                ViewData["listItem"] = checkout;
                return View();
            }
            //get detail
            // formdata[]                
            return RedirectToAction("Index", "Auth");
            return View();
        }
        
            [HttpPost]
        public IActionResult Index(List<int> selectedItems)
        {
            String selectedItem = "";
            foreach (var item in selectedItems)
            {
                 selectedItem = string.Join("/", selectedItems.Select(item => item.ToString()));
            }
            HttpContext.Session.SetString("SelectedItems1", selectedItem);
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
        public IActionResult AddDB(string customerName,string phoneNumber,string address,List<int> listItem1,byte payment_method)
        {
            var customer = HttpContext.Session.GetString("USER_EMAIL");
            var cusId = userDao.GetUserByEmail(customer);
            if (cusId is not null && cusId.RoleId == 2)
            {
                if(customerName == null || phoneNumber == null || address == null)
                {
                    TempData["error_infor"] = "Vui lòng điền đầy đủ thông tin của bạn";
                    return RedirectToAction("Index", "CheckOut" );
                }
                else
                {
                    var check = checkOutDao.adddb(cusId.UserId, customerName, phoneNumber, address, listItem1, payment_method);
                    return RedirectToAction("CustomerView", "Order");
                }
            }
            return RedirectToAction("Index", "Auth");
        }
    }
}
