using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Dao;
using SWP.Dto;
using SWP.Models;

namespace SWP.Controllers
{
    public class AuthController : Controller
    {
        public UsersDao userDao;
		public ManageUsersDao usersManage;
		public AuthController()
        {
            this.userDao = new UsersDao();
            this.usersManage = new ManageUsersDao();
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
                //HttpContext.Session.SetString("USER_EMAIL", );
                HttpContext.Session.SetInt32("USER_ROLE", role);
                HttpContext.Session.SetString("USER_NAME", user.FirstName +" "+ user.LastName);
                if (role == 1)
                {
					return Redirect("/ManageUsers");
				}
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
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View("register", registerModel);
            }
            else
            {
                var check = userDao.Register(registerModel);

                if (check == false)
                {
                    ViewData["message"] = "Email đã tồn tại";
                }
                else
                {
                    await userDao.SendEmailAsync(registerModel.Email, "Đăng ký thanh công", "Chào mừng đến với Boxing Shop!!!");
                    ViewData["message1"] = "Đăng ký thành công.";
                }

                return View();
            }
           
        }

        public IActionResult ForgotPassword()
        {
                return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            try
            {
                var output = userDao.ForgotPassword(email);
                HttpContext.Session.SetString("USER_EMAIL", email);
                if(output != (object)"")
                {
                    ViewData["message"] = "Email không tồn tại!";
                    return View();
                }
                else
                {
                    ViewData["message"] = "";
                }
                return RedirectToAction("inputOtp");
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IActionResult inputOtp()
        {

            return View();

        }

        [HttpPost]
        public IActionResult ConfirmOtp(string email,string otp)
        {
            try
            {
                email = HttpContext.Session.GetString("USER_EMAIL");
                var output = userDao.Confirm(email,otp);
                if(output.Equals("Mã xác nhận hết hạn!"))
                {
                    ViewData["errorType"] = "0"; 
                }
                if(output != "")
                {
                    ViewData["message"] = output;
                }
               
                return RedirectToAction("resetpassword");
            }
            catch (Exception)
            {

                throw;
            }

        }


        public IActionResult resetpassword()
        {
            return View();
        }
            [HttpPost]
        public IActionResult ResetPassword(ResetPasswordModel resetPassword)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("resetpassword");
                }
                else
                {
                    var email = HttpContext.Session.GetString("USER_EMAIL");
                    var output = userDao.resetPassword(email, resetPassword.Password);
                    if (output != "")
                    {
                        ViewData["message"] = output;

                    }
                    else
                    {
                        ViewData["message"] = "Đổi mật khẩu thành công.";
                    }

                    return View();
                }
                
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
