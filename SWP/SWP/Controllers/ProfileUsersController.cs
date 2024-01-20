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
        public ProfileUsersController()
        {
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
        public IActionResult UpdateProfile(UserProfileModel userProfile , string user1)
        {
            string email = HttpContext.Session.GetString("USER_EMAIL");
            user1 = email;
            if (user1 == null)
            {
                return Redirect("Error");
            }
            var user = usersManage.updateProfile(userProfile, user1);
            return Redirect("Index");
        }

    }
}
