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
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace SWP.Controllers
{
    public class BlogController : Controller
    {
        SWPContext context;
        //private readonly BlogDao blogDao;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogController( IWebHostEnvironment webHostEnvironment)
        {
            // blogDao = new BlogDao();
            _webHostEnvironment = webHostEnvironment;
            context= new SWPContext();
        }
        public async Task<List<Blog>> GetBlogs()
        {
                var list = context.Blogs.ToList();
                return list;
            
        }
        public async Task<Blog> GetBlogById(int? id)
        {
            try
            {
                var list = context.Blogs.Where(x => x.BlogId == id).FirstOrDefault();
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Manage(int currentPage)
        {
            var blog = GetBlogs().Result.ToList();
            if(blog != null)
            {
                int startIndex = 6 * currentPage + 1;
                ViewData["NumberOfPages"] = blog.Count / 6;
                blog = blog.Skip(6 * currentPage).Take(6).ToList();
                // Filter out users with Id equal to 1
                // users = users.Where(x => x.RoleId != 1).Skip(6 * currentPage).Take(6).ToList();

                ViewData["currentPage"] = currentPage;
                ViewData["startIndex"] = startIndex;
                return View(blog);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Manage(int currentPage, string search, int? status)
        {
            var blog = GetBlogs().Result.ToList();
            if (blog != null)
            {

                ViewData["NumberOfPages"] = blog.Count / 6;

                if (search != null && !search.Equals("\\s+"))
                {
                    blog = blog.Where(x => (x.Title.ToLower().Trim().Contains(search.ToLower().Trim()) )
                     && (!status.HasValue || x.Status == status)).ToList();

                }
                

                if (status.HasValue)
                {
                    blog = blog.Where(x => x.Status == status).ToList();
                }

                blog = blog.Skip(6 * currentPage).Take(6).ToList();
                int startIndex = 6 * currentPage + 1;

                ViewBag.strSearch = search;
                ViewBag.Status = status;
                ViewData["currentPage"] = currentPage;
                ViewData["startIndex"] = startIndex;
                return View(blog);

            }
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.Error = "";
            return View();
        }

        // POST: Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<IFormFile> files, [Bind("Title,ShortDescription,Description,Status,Image")] Blog blog)
        {
            if (ModelState.IsValid)
            {
               /* int roleId = (int)HttpContext.Session.GetInt32("USER_ROLE");
                int userId = 0;
                if(roleId ==   1) {
                   userId = (int)HttpContext.Session.GetInt32("USER_ID");
                }*/
               
                if (blog == null)
                {
                    return Problem("Blog null");
                }
                string imageArray = "";
                long size = files.Sum(f => f.Length);

                var filePaths = new List<string>();
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var fileName = formFile.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        filePaths.Add("/Images/" + fileName); // Store the relative path to the file
                        imageArray += fileName + ",";
                    }
                }
                

                Blog _blog = new Blog();
                _blog.Title = blog.Title;
               _blog.Description = blog.Description;
                _blog.ShortDescription = blog.ShortDescription;
                _blog.Image = imageArray.Equals("")? "" : imageArray.Substring(0,imageArray.Length - 1);

                if (_blog.Title == null)
                {
                    string error = "Title not null";
                    ViewBag.Error = error;
                    return View();
                }

                context.Blogs.Add(_blog);
                await context.SaveChangesAsync();
               
            }
            return RedirectToAction("Manage");
        }
        public async Task<IActionResult> Details(int? blogId)
        {
            var blog = await GetBlogById(blogId);

            return View(blog);
        }
    }
}
