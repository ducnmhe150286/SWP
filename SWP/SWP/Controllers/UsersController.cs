using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SWP.Dto.Request.Users;
using SWP.Models;
using System.Collections.Generic;
using SWP.Dao;
using Microsoft.Extensions.Hosting;

namespace SWP.Controllers
{

    public class UsersController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly UsersDao usersDao;
        private readonly RoleDao roleDao;
        private readonly IWebHostEnvironment _webHostEnvironment;



        public UsersController(UsersDao usersDao, IWebHostEnvironment environment)
        {
            usersDao = new UsersDao();
            _environment = environment;

        }
        public async Task<List<Role>> GetAllRoles()
        {
            var role = RoleDao.GetAllRoles();
            return role;
        }

        public async Task<List<User>> GetUsers()
        {

            var user = UsersDao.GetAllUser();
            return user;
        }
        public async Task<IActionResult> Index(int currentPage)
        {
            var users = GetUsers().Result.ToList();
            if (users != null)
            {

                int startIndex = 6 * currentPage + 1;
                ViewData["NumberOfPages"] = users.Count / 6;

                users = users.Skip(6 * currentPage).Take(6).ToList();




                // Filter out users with Id equal to 1
                // users = users.Where(x => x.RoleId != 1).Skip(6 * currentPage).Take(6).ToList();

                ViewData["currentPage"] = currentPage;
                ViewData["startIndex"] = startIndex;
                return View(users);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int currentPage, string search, int? status)
        {
            var users = GetUsers().Result.ToList();
            if (users != null)
            {

                ViewData["NumberOfPages"] = users.Count / 6;

                if (search != null && !search.Equals("\\s+"))
                {
                    users = users.Where(x => (x.FirstName.ToLower().Trim().Contains(search.ToLower().Trim()) || x.LastName.ToLower().Trim().Contains(search.ToLower().Trim())
                    || x.PhoneNumber.Trim().Contains(search.Trim()))
                     && (!status.HasValue || x.Status == status)).ToList();

                }


                if (status.HasValue)
                {
                    users = users.Where(x => x.Status == status).ToList();
                }

                users = users.Skip(6 * currentPage).Take(6).ToList();
                int startIndex = 6 * currentPage + 1;

                ViewBag.strSearch = search;
                ViewBag.Status = status;
                ViewData["currentPage"] = currentPage;
                ViewData["startIndex"] = startIndex;
                return View(users);

            }
            return View();
        }


        public async Task<IActionResult> Details(int userId)
        {
            var users = UsersDao.GetUserById(userId);
            var roles = RoleDao.GetAllRoles();

            // Lấy roleName tương ứng với user
            string roleName = roles.FirstOrDefault(r => r.RoleId == users.RoleId)?.RoleName;

            ViewBag.RoleName = roleName;
            return View(users);
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Details(int userId, [Bind("UserId,Status")] UserRequest request)
        {
            var users = UsersDao.GetUserById(userId);
            var roles = RoleDao.GetAllRoles();

            // Lấy roleName tương ứng với user
            string roleName = roles.FirstOrDefault(r => r.RoleId == users.RoleId)?.RoleName;
            users.UserId = userId;
            users.Status = request.Status;


            UsersDao.UpdateUser(users);
            ViewBag.RoleName = roleName;
            return View(users);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int userId, [Bind("UserId,Status")] UserRequest request)

        {
            var users = UsersDao.GetUserById(userId);

            if (users != null)
            {

                users.UserId = userId;
                users.Status = request.Status;


                UsersDao.UpdateUser(users);
            }
            var roles = RoleDao.GetAllRoles();

            // Lấy roleName tương ứng với user
            string roleName = roles.FirstOrDefault(r => r.RoleId == users.RoleId)?.RoleName;

            ViewBag.RoleName = roleName;
            return View(users);
        }

        public IActionResult Add(int userId)
        {
            // Trả về view để hiển thị form thêm mới
            // Nếu bạn muốn truyền dữ liệu khác vào view, bạn có thể thực hiện ở đây
            ViewBag.Role = RoleDao.GetAllRoles();
            ViewBag.Status = UsersDao.GetAllUser();
            return View();
        }

        // POST: Users/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add([Bind("Email,RoleId,FirstName,LastName,PhoneNumber,Status")] CreateUserRequest request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo một đối tượng User từ UserRequest
                    var user = new User
                    {
                        Email = request.Email,
                        RoleId = request.RoleId,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        PhoneNumber = request.PhoneNumber,

                        Status = request.Status,
                        CreatedDate = DateTime.Now,

                    };

                    // Thêm user vào cơ sở dữ liệu
                    UsersDao.SaveUser(user);

                    // Chuyển hướng người dùng đến trang danh sách hoặc trang chi tiết của user vừa tạo
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi thêm user. Vui lòng thử lại sau.");

                    // Nếu bạn muốn ghi log lỗi, bạn có thể sử dụng logging framework như Serilog hoặc log vào cơ sở dữ liệu
                    // _logger.LogError(ex, "Lỗi khi thêm user");
                }
            }

            // Nếu ModelState không hợp lệ, trả về view với các lỗi
            ViewBag.Role = RoleDao.GetAllRoles();
            ViewBag.Status = UsersDao.GetAllUser();
            return View(request);
        }






        public IActionResult Delete(int userId)
        {

            var userid = UsersDao.GetUserById(userId);
            if (userid != null)
            {
                UsersDao.DeleteUser(userid);
            }
            return RedirectToAction("Index");
        }


    }
}
