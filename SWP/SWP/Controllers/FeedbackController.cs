using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Dao;
using SWP.Dto;
using SWP.Dto.Request.Blogs;
using SWP.Models;
using System.Reflection.Metadata;

namespace SWP.Controllers
{
    public class FeedbackController : Controller
    {
        SWPContext context;
        public UsersDao usersDao;
        private readonly IWebHostEnvironment _environment;
        public FeedbackController( IWebHostEnvironment environment, UsersDao usersDao)
        {
            context = new SWPContext();
            _environment = environment;
            this.usersDao = usersDao;
        }
        public async Task<List<FeedBack>> GetFeedBack()
        {
            var list = context.FeedBacks.ToList();
            return list;

        }
        public async Task<FeedBack> GetFeedBackById(int? id)
        {
            try
            {
                var list = context.FeedBacks.Where(x => x.FeedBackId == id).FirstOrDefault();
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == HttpContext.Session.GetString("USER_EMAIL"));
            if (user == null || user.RoleId != 1) // Nếu không phải là Admin
            {
                return RedirectToAction("Index", "Auth"); // Chuyển hướng đến trang đăng nhập
            }
            var feedbacks = context.FeedBacks.Include(f => f.User).Include(d => d.Product).ToList();
            if (feedbacks != null)
            {
                // Tính toán số lượng phản hồi và số trang
                int totalFeedbacks = feedbacks.Count();
                int totalPages = (int)Math.Ceiling((double)totalFeedbacks / pageSize);

                // Lấy phản hồi cho trang hiện tại
                var feedbacksForPage = feedbacks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View(feedbacksForPage);
            }
            return View();
        }

        /* public ActionResult CustomerComment(int productId, LoginModel loginModel)
         {
             var user = usersDao.login(loginModel);
             var data = new FeedbackDao().ListFeedbackViewModel(productId);
             HttpContext.Session.SetInt32("USER_ID", user.UserId);
         }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productId, string reviewContent, int rating)
        {
            try
            {
                string email = HttpContext.Session.GetString("USER_EMAIL");
                if(email == null)
                {
                    return RedirectToAction("Index", "Auth");
                }
                User user = context.Users.SingleOrDefault(n => n.Email == email);
                var data = new FeedbackDao().ListFeedbackViewModel(productId);
                var newFeedback = new FeedBack
                {
                    Rating = rating,
                    Comment = reviewContent,
                    ProductId = productId, // Liên kết với sản phẩm
                    UserId = user.UserId, // Liên kết với người dùng
                    CreatedDate = DateTime.Now
                };
                // Thêm đối tượng phản hồi mới vào cơ sở dữ liệu và lưu thay đổi
                context.FeedBacks.Add(newFeedback);
                await context.SaveChangesAsync();

                // Điều hướng người dùng đến trang chi tiết sản phẩm sau khi thêm phản hồi thành công
                return RedirectToAction("Index", "ProductDetail", new { productId = productId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "ProductDetail", new { productId = productId });
            }
           
        }
      

        public ActionResult Delete(int feedbackID)
        {
            FeedBack feedBack = context.FeedBacks.SingleOrDefault(n => n.FeedBackId == feedbackID);
            context.Remove(feedBack);
            context.SaveChanges();
            return RedirectToAction("Index", "ProductDetail", new { productId = feedBack.ProductId });
        }

        public ActionResult Edit(int id, string reviewContent, int rating)
        {
            FeedBack feedBack = context.FeedBacks.SingleOrDefault(n => n.FeedBackId == id);
            feedBack.Comment = reviewContent;
            feedBack.Rating = rating;
            context.SaveChanges();
            return RedirectToAction("Index", "ProductDetail", new { productId = feedBack.ProductId });
        }




        
    }
}
