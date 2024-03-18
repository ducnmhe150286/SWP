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
using System.Diagnostics;

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
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
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
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
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
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            var users = UsersDao.GetUserById(userId);
            var roles = RoleDao.GetAllRoles();

            
            // Lấy roleName tương ứng với user
            string roleName = roles.FirstOrDefault(r => r.RoleId == users.RoleId)?.RoleName;
            users.UserId = userId;
            users.Status = request.Status;
            // Kiểm tra xem người dùng hiện tại có phải là admin hay không
            bool isAdmin = roleName.ToLower() == "admin";

            // Nếu không phải admin và đang cố gắng cập nhật trạng thái, từ chối
            if (!isAdmin && request.Status == users.Status)
            {
                // Redirect hoặc thông báo lỗi tùy thuộc vào logic của bạn
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái khách hàng";
                return RedirectToAction("Index"); // Hoặc trả về một ActionResult khác
            }
            TempData["SuccessMessage"] = "cập nhật thành công";
            UsersDao.UpdateUser(users);
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
        public IActionResult Add([Bind("Email,RoleId,FirstName,LastName,Password,PhoneNumber,Status")] CreateUserRequest request)
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
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
                        Password= request.Password,
                        CreatedDate = DateTime.Now,

                    };
                    TempData["SuccessMessage"] = "Tạo mới thành công";
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
