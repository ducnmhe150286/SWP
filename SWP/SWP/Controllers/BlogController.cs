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
            return View();
        }

        // POST: Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,ShortDescription,Description,Status,Image")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                /*if (blog.Image != null && blog.Image.Length > 0)
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var imageName = Guid.NewGuid().ToString() + "_" + blog.Image.FileName;
                    var fullPath = Path.Combine(imagePath, imageName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await blog.Image.CopyToAsync(fileStream);
                    }

                    blog.Image = "/images/" + imageName;
                }

                context.Add(blog);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));*/
            }
            return RedirectToAction("Manage");
        }
        public async Task<IActionResult> Details(int? id)
        {
            var blog = await GetBlogById(id);

            return View(blog);
        }
    }
}
