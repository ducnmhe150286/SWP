using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Dao;
using SWP.Dto;

namespace SWP.Controllers
{
    public class AuthController : Controller
    {
        public UsersDao userDao;

        public AuthController()
        {
            this.userDao = new UsersDao();
        }

        public IActionResult Index(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                return View();
            }
            var user = userDao.login(email, password);
            if (user == null)
            {
                ViewData["error"] = "Mat khau hoac email sai";
            }
            else
            {
                int role = (int)user.RoleId;
                HttpContext.Session.SetString("USER_EMAIL", user.Email);
                HttpContext.Session.SetInt32("USER_ROLE", role);
                HttpContext.Session.SetString("USER_NAME", user.FirstName +" "+ user.LastName);
                return Redirect("/Home");
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid && ModelState == null)
            {
                registerModel.ModelState = ModelState;
                return View("register", registerModel);
            }

            var check = userDao.Register(registerModel);

            if (check == false)
            {
                ViewData["message"] = "Email đã tồn tại";
            }
            else
            {
                ViewData["message1"] = "Đăng ký thành công.";
            }

            return View();
        }
    }
}
