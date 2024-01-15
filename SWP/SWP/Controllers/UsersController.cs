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

        public UsersController( UsersDao usersDao)
        {
            usersDao = new UsersDao();
           
        }

       
        public async Task<List<User>> GetUsers()
        {
           
            var user = UsersDao.GetAllUser();
            return user ;
        }
        public async Task<IActionResult> Index()
        {
            return View(GetUsers().Result.ToList());
        }
        /* public async Task<List<Role>> GetRoles()
         {

         }
         // GET: Users


         // GET: Users/Details/5
         public async Task<IActionResult> Details(int userId)
         {

         }



         // GET: Users/Edit/5
         public async Task<IActionResult> Edit(int userId)
         {

         }

         // POST: Users/Edit/5
         // To protect from overposting attacks, enable the specific properties you want to bind to.
         // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
         [HttpPost]
         [ValidateAntiForgeryToken]
        *//* public async Task<IActionResult> Edit(int userId,List<IFormFile> files, [Bind("UserId,RoleId,Email,Password,Address,PhoneNumber,FirstName,LastName,Gender,Status,CreatedDate,CreatedBy,UpdatedBy,Image,Otp,OtpExpired")] User user)
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
         }*//*

         // GET: Users/Delete/5
         public async Task<IActionResult> Delete(int? id)
         {

         }

         // POST: Users/Delete/5
         [HttpPost, ActionName("Delete")]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> DeleteConfirmed(int id)
         {
             return View();
         }
 */

    }
}
