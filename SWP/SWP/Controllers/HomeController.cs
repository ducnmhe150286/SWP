using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Dao;
using SWP.Models;
using SWP.Utilities;
using System.Diagnostics;

namespace SWP.Controllers
{
	public class HomeController : Controller
	{
        private readonly ILogger<HomeController> _logger;
        public UsersDao usersDao;

        public HomeController(ILogger<HomeController> logger)
        {
            usersDao = new UsersDao();
            _logger = logger;

        }


        public ActionResult Index(int? categoryFilter, int? brandFilter, string searchString, int page = 1)
        {
            int pageSize = 12;

            using (var context = new SWPContext())
            {
                var brands = context.Brands.Where(b => b.Status == 1).ToList();
                var categories = context.Categories.Where(c => c.Status == 1).ToList();

                var query = context.Products
                    .Include(p => p.ProductImages)
                    .Where(p => p.Status == 1);

                // Apply filters based on user input
                if (categoryFilter.HasValue)
                {
                    int categoryId = categoryFilter.Value;
                    query = query.Where(p => p.CategoryId == categoryId);
                }

                if (brandFilter.HasValue)
                {
                    int brandId = brandFilter.Value;
                    query = query.Where(p => p.BrandId == brandId);
                }

                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    query = query.Where(p => p.ProductName.ToLower().Contains(searchString));
                }

                var productList = query.ToList();
                if (productList.Count == 0)
                {
                    TempData["ErrorMessage"] = "Không có dữ liệu sản phẩm.";
                }
                var paginatedList = PaginatedList<Product>.Create(productList.AsQueryable(), page, pageSize);

                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                ViewBag.ProductList = paginatedList;
                ViewBag.CategoryFilter = categoryFilter;
                ViewBag.BrandFilter = brandFilter;
                ViewData["ErrorMessage"] = TempData["ErrorMessage"] as string;
                ViewData["SuccessMessage"] = TempData["SuccessMessage"] as string;
            }

            // Pass the search string back to the view
            ViewData["SearchString"] = searchString;

            return View();
        }


        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
        
    }
}