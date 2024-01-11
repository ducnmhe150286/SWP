using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Dao;

namespace SWP.Controllers
{
    public class AuthController : Controller
    {
        public UsersDao userDao;

        public AuthController()
        {
            this.userDao = new UsersDao();
        }


       
     
       public IActionResult Index(string email,string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                return View();
            }
            var user=userDao.login(email,password);
            if(user == null)
            {
                ViewData["error"] = "Mat khau hoac email sai";
            }
            else
            { 
                int role=(int)user.RoleId;
                HttpContext.Session.SetString("USER_EMAIL", user.Email);
                HttpContext.Session.SetInt32("USER_ROLE", role);

                return Redirect("/Home");
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
