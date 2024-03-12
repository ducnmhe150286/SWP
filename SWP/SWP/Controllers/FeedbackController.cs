using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Dto;
using SWP.Models;
using System.Reflection.Metadata;

namespace SWP.Controllers
{
    public class FeedbackController : Controller
    {
        SWPContext context;
        private readonly IWebHostEnvironment _environment;
        public FeedbackController( IWebHostEnvironment environment)
        {
            context = new SWPContext();
            _environment = environment;
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

        public IActionResult Index(int currentPage)
        {
            var feedbacks = context.FeedBacks
                    .Include(f => f.User)
                    .Include(f => f.Detail)
                    .ThenInclude(d => d.Product).ToList();
            if (feedbacks != null)
            {
                int startIndex = 6 * currentPage + 1;
                ViewData["NumberOfPages"] = feedbacks.Count / 6;
                feedbacks = feedbacks.Skip(6 * currentPage).Take(6).ToList();
                ViewData["currentPage"] = currentPage;
                ViewData["startIndex"] = startIndex;
                return View(feedbacks);
            }
            var displayModels = feedbacks.Select(feedback => new FeedbackViewModel
            {
                FeedBackId = feedback.FeedBackId,
                Email = feedback.User?.Email,
                ProductName = feedback.Detail?.Product?.ProductName,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                CreatedDate = feedback.CreatedDate ?? DateTime.MinValue
            }).ToList();
            ViewBag.DisplayModels = displayModels;
            
            return View();
        }
       /* public async Task<IActionResult> Create(int productId)
        {
            // Kiểm tra đăng nhập
            if (User.Identity.IsAuthenticated)
            {
                // Lấy thông tin người dùng hiện tại (giả sử là email)
                var userEmail = User.Identity.Name;
                // Kiểm tra người dùng đã mua sản phẩm chưa
                var userHasPurchased = context.Orders.Any(o => o.ProductId == productId && o.UserEmail == userEmail);
                if (userHasPurchased)
                {
                    // Nếu đã mua, hiển thị form đánh giá hoặc tiếp tục logic
                    return View();
                }
                else
                {
                    // Nếu chưa mua, thông báo hoặc chuyển hướng
                    ViewBag.Error = "Bạn cần mua sản phẩm này trước khi đánh giá.";
                    return View("Error"); // Giả sử có View Error
                }
            }
            else
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }
        }*/

        public IActionResult Update()
        {

            return View();
        }
        public IActionResult Delete()
        {

            return View();
        }
    }
}
