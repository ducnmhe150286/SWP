using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SWP.Dao;
using SWP.Dto;
using SWP.Models;

namespace SWP.Controllers
{
    public class ProfileUsersController : Controller
    {
        public ManageUsersDao usersManage;
        private readonly IWebHostEnvironment _environment;
        public ProfileUsersController(IWebHostEnvironment environment)
        {
            _environment = environment;
            usersManage = new ManageUsersDao();
        }
        
        public IActionResult Index()
        {
            string email = HttpContext.Session.GetString("USER_EMAIL");
            if (email == null)
            {
                return Redirect("Error");
            }
            
            var user = usersManage.getUserByEmail(email);
            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(UserProfileModel userProfile)
        {
            string email = HttpContext.Session.GetString("USER_EMAIL");
            
            if (email == null)
            {
                return RedirectToAction("Error");
            }
            var user = usersManage.updateProfile(userProfile, email);
            HttpContext.Session.SetString("USER_NAME", user.FirstName + " " + user.LastName);
            return Redirect("Index");
        }
     
        public IActionResult ChangePassword()
        {
            string email = HttpContext.Session.GetString("USER_EMAIL");

            if (email == null)
            {
                return RedirectToAction("Error");
            }
           
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePassWordModel passWordModel)
        {
            string email = HttpContext.Session.GetString("USER_EMAIL");

            if (email == null)
            {
                return RedirectToAction("Error");
            }
            if (!ModelState.IsValid)
            {
                return View("ChangePassword", passWordModel);
            }
            var user = usersManage.changePassWord(passWordModel,email);
            if (user != "")
            {
                TempData["MESS_NOTE_ERROR"] = "Đổi mậu khẩu không thành công!";

            }
            else
            {
                TempData["MESS_NOTE_SUCCESS"] = "Đổi mật khẩu thành công.";
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            string email = HttpContext.Session.GetString("USER_EMAIL");

            if (email == null)
            {
                return RedirectToAction("Error");
            }

            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var fileExtension = Path.GetExtension(file.FileName);

                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                var newFileName = $"{fileName}_{timestamp}{fileExtension}";

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var filePath = Path.Combine(uploadsFolder, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var checkaddimage = usersManage.checkAddImage(newFileName, email);

                if (checkaddimage == false)
                {
                    TempData["MESS_NOTE"] = "Thêm ảnh không thành công";
                }
                else
                {
                    TempData["MESS_NOTE"] = "Thêm ảnh thành công";
                }

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Vui lòng chọn tệp tin để tải lên.";
                return View("Error");
            }
        }

    }
}
