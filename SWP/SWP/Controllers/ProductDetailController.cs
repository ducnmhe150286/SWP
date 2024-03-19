﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Dao;
using SWP.Dto;
using SWP.Models;

namespace SWP.Controllers
{
    public class ProductDetailController : Controller
    {
        public ProductDao proDetail;
        public UsersDao usersDao;
        private SWPContext context;
        public ProductDetailController()
        {
            this.proDetail = new ProductDao();
            this.usersDao = new UsersDao();
            context = new SWPContext();
        }
        public IActionResult Index(int productId)
        {
            if(productId == 0)
            {
                productId = (int)TempData["productId"];
            }
            string email = HttpContext.Session.GetString("USER_EMAIL");
            var pro = proDetail.getProductById(productId);
            var listProSimilar = proDetail.getProductByCategory(pro);
            bool check = email != null && context.Orderdetails.Where(n => n.Detail.ProductId == productId && n.Order.User.Email == email).Count() > 0 
                && context.FeedBacks.Where(n => n.Product.ProductId == productId && n.User.Email == email).Count() == 0;
            ViewData["listPro"] = listProSimilar;
            ViewData["error_quantity"] = TempData["error_quantity"];
            List<FeedBack> feedBacks = context.FeedBacks.Include(n => n.User).Where(n => n.Product.ProductId == productId).ToList();
            ViewData["listPro"] = listProSimilar;
            ViewBag.check = check;
            ViewBag.listFeedback = feedBacks;
            if(feedBacks != null && feedBacks.Count() != 0)
            {
                ViewBag.isUser = email == feedBacks[0].User.Email;
            }
            else
            {
                ViewBag.isUser = false; 
            }



            return View(pro);
        }
        
    }
}
