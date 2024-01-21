//using Microsoft.AspNetCore.Mvc;
//using SWP.Dao;
//using SWP.Models;

//namespace SWP.Controllers
//{
//    public class ManageUsersController : Controller
//    {
//        public ManageUsersDao usersManage;
//        public ManageUsersController()
//        {
//            usersManage = new ManageUsersDao();
//        }
//        public IActionResult Index()
//        {
//           string email= HttpContext.Session.GetString("USER_EMAIL");

//            if (usersManage.CheckAdmin(email) == false)
//            {
//                return Redirect("Error");
//            }
//            var listuser = usersManage.GetAllUsers();
//            return View(listuser);
//        }

//        public User GetUserDetails(int userId)
//        {
//            string email = HttpContext.Session.GetString("USER_EMAIL");

//            if (usersManage.CheckAdmin(email) == false)
//            {
//                 Redirect("Error");
//            }
//            var user = usersManage.getUser(userId);
//            return user;
//        }
//        public IActionResult GetUserBlock(int? id, int? block)
//        {
//            string email = HttpContext.Session.GetString("USER_EMAIL");

//            if (usersManage.CheckAdmin(email) == false)
//            {
//                return Redirect("Error");
//            }
//            if (id != null && block != null)
//            {

//                var check = usersManage.BlockUser(id, block,email);
//                TempData["MESS_NOTE"] = check;
//                return RedirectToAction("Index");
//            }
//            return RedirectToAction("Index");
//        }

//	}
//}
