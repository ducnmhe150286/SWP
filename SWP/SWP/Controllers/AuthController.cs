using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        public IActionResult Index()
        {
            string successMessage = TempData["SuccessMessage"] as string;
            if (successMessage != null && successMessage != "")
            {
                TempData["MESS_NOTE_SUCCESS"]= successMessage;
            }
            return View();
        }
        [HttpPost]
        public IActionResult Index(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", loginModel);
            }
            var user = userDao.login(loginModel);
            if (user == null)
            {
                ViewData["error"] = "Email hoặc mật khẩu không đúng!";
            }
            else
            {
                if (user.Status != 1)
                {
                    // Nếu status không phải là 1, tức là không có quyền truy cập, 
                    // bạn có thể hiển thị thông báo hoặc thực hiện hành động phù hợp khác ở đây.
                    ViewData["error"] = "Tài khoản của bạn đã dừng hoạt động!";
                    return View();
                }
                int role = (int)user.RoleId;
                HttpContext.Session.SetString("USER_EMAIL", user.Email);
                //HttpContext.Session.SetString("USER_EMAIL", );
                HttpContext.Session.SetInt32("USER_ROLE", role);
                HttpContext.Session.SetString("USER_NAME", user.FirstName +" "+ user.LastName);
                HttpContext.Session.SetInt32("USER_ID", user.UserId);
                if (role == 1)
                {
					return Redirect("/Dashboard");
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
                TempData["SuccessMessage"] = "Đăng ký thành công.";
                return RedirectToAction("Index");
            }
           
        }

        public IActionResult ForgotPassword()
        {
                return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                var output =await userDao.ForgotPassword(email);
                HttpContext.Session.SetString("USER_EMAIL", email);
                if(output !=true)
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
                //if(output.Equals("Mã xác nhận hết hạn!"))
                //{
                //    ViewData["errorType"] = "0";
                //    return View("inputOtp");
                //}
                if(output != "")
                {
                    ViewData["errorType"] = output;
                    
                    return View("inputOtp");
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
