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
using RestSharp;
using SWP.Dto.Request.Users;
using SWP.Models;
using System.Collections.Generic;
using SWP.Dao;

namespace SWP.Controllers
{
   
    public class UsersController : Controller
    {
        
        private readonly UsersDao usersDao;
        private readonly RoleDao roleDao;

        public UsersController( UsersDao usersDao)
        {
            usersDao = new UsersDao();
           
        }
        public async Task<List<Role>> GetAllRoles()
        { 
            var role = RoleDao.GetAllRoles();
            return role;
        }

        public async Task<List<User>> GetUsers()
        {
           
            var user = UsersDao.GetAllUser();
            return user ;
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
                    users = users.Where(x => x.FirstName.ToLower().Trim().Contains(search.ToLower().Trim()) || x.LastName.ToLower().Trim().Contains(search.ToLower().Trim()) 
                    || x.PhoneNumber.Trim().Contains(search.Trim()) || x.Status.ToString().Trim().Contains(search.Trim()) || (x.Status == 1 && search.ToLower().Trim() == "active") || (x.Status != 1 && search.ToLower().Trim() == "deactive")).ToList();
                   
                   
                }
                
                if (status.HasValue && status.Value != 0)
                {

                    if (status.Value == 1) // Active
                    {
                        users = users.Where(x => x.Status == 1).ToList();
                    }
                    else if (status.Value != 1) // Deactive
                    {
                        users = users.Where(x => x.Status != 1).ToList();
                    }
                }

                users = users.Skip(6 * currentPage).Take(6).ToList();
                int startIndex = 6 * currentPage + 1;

                ViewBag.strSearch = search;
                ViewData["currentPage"] = currentPage;
                ViewData["startIndex"] = startIndex;
                return View(users);

            }
            return View();
        }
            /*[HttpPost]
        public async Task<IActionResult> Index(int currentPage, string search, int? status)
        {
            var users = await GetUsers();

            if (users != null)
            {
                // Apply search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    users = users.Where(x =>
                        x.FirstName.ToLower().Trim().Contains(search.ToLower().Trim()) ||
                        x.LastName.ToLower().Trim().Contains(search.ToLower().Trim()) ||
                        x.PhoneNumber.Contains(search) ||
                        x.Status.ToString().Contains(search)
                    ).ToList();
                }

                // Apply status filter
                if (status.HasValue)
                {
                    users = users.Where(x => x.Status == status).ToList();
                }

                // Pagination logic
                int startIndex = 6 * currentPage + 1;
                ViewData["NumberOfPages"] = (int)Math.Ceiling((double)users.Count / 6);

                // Apply pagination
                users = users.Skip(6 * currentPage).Take(6).ToList();

                ViewData["currentPage"] = currentPage;
                ViewData["startIndex"] = startIndex;

                return View(users);
            }

            return View();
        }
*/

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
        /*public async Task<IActionResult> Edit(int userId)
        {
            var users = UsersDao.GetUserById(userId);
            ViewBag.Role = RoleDao.GetAllRoles();
            ViewBag.Status = UsersDao.GetAllUser();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int userId, [Bind("UserId,Status")] UserRequest request)
        {
            var users = UsersDao.GetUserById(userId);
            if (users != null)
            {
                users.UserId= userId;
                //users.Address = request.Address;
                //users.PhoneNumber= request.PhoneNumber;
                //users.LastName = request.LastName;
                //users.FirstName = request.FirstName;
                //users.Email = request.Email;
                //users.RoleId = request.RoleId;
                //users.Password= request.Password;
                //users.Image= request.Image;
                //users.Gender= request.Gender;
                users.Status= request.Status;
                UsersDao.UpdateUser(users);
            }
           
            return RedirectToAction("Index");
        }*/
       
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
