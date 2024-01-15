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

namespace SWP.Controllers
{
   
    public class UsersController : Controller
    {
     
        private readonly HttpClient client = null;
        private readonly IConfiguration configuration;
        private string ApiPort = "";
        private readonly INotyfService _toastNotification;

        public UsersController( IConfiguration configuration, INotyfService toastNotification)
        {
           
            client= new HttpClient();
            var contenType = new MediaTypeWithQualityHeaderValue("application/json");
            this.configuration = configuration;
            _toastNotification = toastNotification;
            ApiPort = configuration.GetSection(ApiPort).Value;
        }

        public async Task<UserRequest> GetCurrentUser()
        {
            var user = HttpContext.Session.GetString("currentuser");
            if(user!= null)
            {
                var currentUser = JsonConvert.DeserializeObject<UserRequest>(user);
                return currentUser;
            }
            return null;
        }

        public async Task<List<User>> GetUsers()
        {
            var user = GetCurrentUser().Result;
            if(user.RoleId == 1)
            {
                try
                {
                    var token = HttpContext.Session.GetString("AuthToken");
                    var tokenAuth = "Bearer" + token;

                    RestClient client = new RestClient(ApiPort);
                    var requestUrl = new RestRequest($"api/Users", RestSharp.Method.Get);
                    requestUrl.AddHeader("content-type", "application/json");
                    requestUrl.AddParameter("Authorization", tokenAuth.Replace("\"",""), ParameterType.HttpHeader);
                    var response = await client.ExecuteAsync(requestUrl);
                    var products = JsonConvert.DeserializeObject<List<User>>(response.Content);
                    if(products != null)
                    {
                        return products;
                    }
                }catch (Exception ex)
                {
                    throw;
                }
              
            }
            return null;
        }

        public async Task<List<Role>> GetRoles()
        {
            try
            {
                
                RestClient client = new RestClient(ApiPort);
                var requestUrl = new RestRequest($"api/Users", RestSharp.Method.Get);
                requestUrl.AddHeader("content-type", "application/json");
                var response = await client.ExecuteAsync(requestUrl);
                var roles = JsonConvert.DeserializeObject<List<Role>>(response.Content);
                return roles;
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        // GET: Users
        public async Task<IActionResult> Index(int currentPage)
        {
           var user = GetCurrentUser().Result;
            if (user.RoleId == 1)
            {
                var session = HttpContext.Session.GetString("currentuser");
                if(session != null)
                {
                    var currentUser = JsonConvert.DeserializeObject<User>(session);
                    ViewData["Name"] = currentUser.LastName;
                    ViewData["Role"] = currentUser.RoleId;
                }

                var users = GetUsers().Result;
                if(user != null)
                {
                    ViewData["NumberofPages"] = users.Count / 6;
                    users = users.Skip( 6 +currentPage).Take(6).ToList();
                    ViewData["currentPage"] = currentPage;
                    return View(users);
                }
            }
            _toastNotification.Error("Ban khong co quyen truy cap trang nay");
            return View();
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int userId)
        {
            var session = HttpContext.Session.GetString("currentuser");
            if (session != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(session);
                ViewData["Name"] = currentUser.LastName;
                ViewData["Role"] = currentUser.RoleId;

                var token = HttpContext.Session.GetString("AuthToken");
                var tokenAuth = "Bearer" + token;

                RestClient client = new RestClient(ApiPort);
                var requestUrl = new RestRequest($"api/Users", RestSharp.Method.Get);
                requestUrl.AddHeader("content-type", "application/json");
                requestUrl.AddParameter("Authorization", tokenAuth.Replace("\"", ""), ParameterType.HttpHeader);
                var response = await client.ExecuteAsync(requestUrl);
                var userid = JsonConvert.DeserializeObject<User>(response.Content);
                if (userid != null)
                {
                    return View(userid);
                }
            }
            var user = GetCurrentUser().Result;
            if (user.RoleId == 1)
            {
              
                    var token = HttpContext.Session.GetString("AuthToken");
                    var tokenAuth = "Bearer" + token;

                    RestClient client = new RestClient(ApiPort);
                    var requestUrl = new RestRequest($"api/Users", RestSharp.Method.Get);
                    requestUrl.AddHeader("content-type", "application/json");
                    requestUrl.AddParameter("Authorization", tokenAuth.Replace("\"", ""), ParameterType.HttpHeader);
                    var response = await client.ExecuteAsync(requestUrl);
                    var userid = JsonConvert.DeserializeObject<User>(response.Content);
                    if (userid != null)
                    {
                        return View(userid);
                }
                
            }
            _toastNotification.Error("Ban khong co quyen truy cap trang nay");
            return View();
        }



        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int userId)
        {
            var session = HttpContext.Session.GetString("currentuser");
            if (session != null)
            {
                var currentUser = JsonConvert.DeserializeObject<User>(session);
                ViewData["Name"] = currentUser.LastName;
                ViewData["Role"] = currentUser.RoleId;

                var token = HttpContext.Session.GetString("AuthToken");
                var tokenAuth = "Bearer" + token;

                RestClient client = new RestClient(ApiPort);
                var requestUrl = new RestRequest($"api/Users", RestSharp.Method.Get);
                requestUrl.AddHeader("content-type", "application/json");
                requestUrl.AddParameter("Authorization", tokenAuth.Replace("\"", ""), ParameterType.HttpHeader);
                var response = await client.ExecuteAsync(requestUrl);
                var user = JsonConvert.DeserializeObject<User>(response.Content);
                if (user != null)
                {
                    var listRole = GetRoles();
                    ViewData["RoleId"] = new SelectList(listRole.Result.ToList(), "RoleId", "RoleName");
                    return View(user);
                }
            }
            _toastNotification.Error("Ban khong co quyen truy cap trang nay");
            return View();
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       /* public async Task<IActionResult> Edit(int userId,List<IFormFile> files, [Bind("UserId,RoleId,Email,Password,Address,PhoneNumber,FirstName,LastName,Gender,Status,CreatedDate,CreatedBy,UpdatedBy,Image,Otp,OtpExpired")] User user)
        {
            var filePaths = new List<string>();
            if (files.Count > 0)
            {
                long size = files.Sum(f => f.Length);

                foreach(var formFile in files)
                {
                    if(formFile.Length > 0)
                    {
                        var fileName = formFile.FileName;
                        var filePath = Path.Combine(Dictionary.GetCurrentDirectory(), "wwwroot", "image");
                    }
                }
            }
        }*/

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = GetCurrentUser().Result;
            if (user.RoleId == 1)
            {

                var token = HttpContext.Session.GetString("AuthToken");
                var tokenAuth = "Bearer" + token;
                try
                {
                    RestClient client = new RestClient(ApiPort);
                    var requestUrl = new RestRequest($"api/Users", RestSharp.Method.Get);
                    requestUrl.AddHeader("content-type", "application/json");
                    requestUrl.AddParameter("Authorization", tokenAuth.Replace("\"", ""), ParameterType.HttpHeader);
                    var response = await client.ExecuteAsync(requestUrl);
                    if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        _toastNotification.Success("Delete user success!");
                        return RedirectToAction("Index", "User");
                    }
                }catch (Exception ex) {
                    throw;
                }

               

            }
            _toastNotification.Error("Xoa nguoi dung that bai");
            return RedirectToAction("Index", "User");
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return View();
        }

       
    }
}
