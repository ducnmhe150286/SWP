using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Dao;
using SWP.Models;
using SWP.Utilities;
using System.Diagnostics;
using System.Globalization;
using System.Text;

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


        public ActionResult Index(int? categoryFilter, int? brandFilter, string searchString, int productPage = 1, int blogPage = 1)
        {
            int productPageSize = 12; // Set the page size for products
            int blogPageSize = 4; // Set the page size for the blog


            using (var context = new SWPContext())
            {
                var blog = context.Blogs.Where(b =>b.Status == 1).ToList();
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

                //if (!string.IsNullOrEmpty(searchString))
                //{
                //    searchString = searchString.ToLower();
                //    query = query.Where(p => p.ProductName.ToLower().Contains(searchString));
                //}
                // ...

                if (!string.IsNullOrEmpty(searchString))
                {
                    var combinedSearchString = RemoveDiacritics(searchString.ToLower());
                    var searchWords = RemoveDiacritics(searchString.ToLower()).Split(' ');

                    query = query.AsEnumerable()
                                 .Where(p => RemoveDiacritics(p.ProductName.ToLower()).Contains(combinedSearchString) ||
                                             searchWords.All(word => RemoveDiacritics(p.ProductName.ToLower()).Contains(word)))
                                 .AsQueryable();

                }

                var productList = query.OrderByDescending(p => p.CreatedDate).ToList();
                if (productList.Count == 0)
                {
                    TempData["ErrorMessage"] = "Không có dữ liệu sản phẩm.";
                }
                var paginatedList = PaginatedList<Product>.Create(productList.AsQueryable(), productPage, productPageSize);

                // Pagination for Blog
                var blogPaginatedList = PaginatedList<Blog>.Create(blog.AsQueryable(), blogPage, blogPageSize);
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                ViewBag.Blog = blogPaginatedList; // Use the paginated blog list here
                ViewBag.ProductList = paginatedList;
                ViewBag.CategoryFilter = categoryFilter;
                ViewBag.BrandFilter = brandFilter;
                ViewData["ErrorMessage"] = TempData["ErrorMessage"] as string;
                ViewData["SuccessMessage"] = TempData["SuccessMessage"] as string;
            }

            // Pass the search string back to the view
            ViewData["SearchString"] = searchString;
            ViewData["ProductPage"] = productPage;
            ViewData["BlogPage"] = blogPage;
            return View();
        }
        public ActionResult ListProduct(int? categoryFilter, int? brandFilter, string searchString, int productPage = 1, int blogPage = 1)
        {
            int productPageSize = 12; // Set the page size for products


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

                //if (!string.IsNullOrEmpty(searchString))
                //{
                //    searchString = searchString.ToLower();
                //    query = query.Where(p => p.ProductName.ToLower().Contains(searchString));
                //}
                // ...

                if (!string.IsNullOrEmpty(searchString))
                {
                    var combinedSearchString = RemoveDiacritics(searchString.ToLower());
                    var searchWords = RemoveDiacritics(searchString.ToLower()).Split(' ');

                    query = query.AsEnumerable()
                                 .Where(p => RemoveDiacritics(p.ProductName.ToLower()).Contains(combinedSearchString) ||
                                             searchWords.All(word => RemoveDiacritics(p.ProductName.ToLower()).Contains(word)))
                                 .AsQueryable();

                }

                var productList = query.OrderByDescending(p => p.CreatedDate).ToList();
                if (productList.Count == 0)
                {
                    TempData["ErrorMessage"] = "Không có dữ liệu sản phẩm.";
                }
                var paginatedList = PaginatedList<Product>.Create(productList.AsQueryable(), productPage, productPageSize);
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
            ViewData["ProductPage"] = productPage;
            ViewData["BlogPage"] = blogPage;
            return View();
        }
        public ActionResult ListBlog(int? categoryFilter, int? brandFilter, string searchString, int productPage = 1, int blogPage = 1)
        {
            int blogPageSize = 4; // Set the page size for the blog

            using (var context = new SWPContext())
            {
                var blog = context.Blogs.Where(b => b.Status == 1).OrderByDescending(p =>p.CreateDate).ToList();
                // Pagination for Blog
                var blogPaginatedList = PaginatedList<Blog>.Create(blog.AsQueryable(), blogPage, blogPageSize);
                ViewBag.Blog = blogPaginatedList; // Use the paginated blog list here
                ViewBag.CategoryFilter = categoryFilter;
                ViewBag.BrandFilter = brandFilter;
                ViewData["ErrorMessage"] = TempData["ErrorMessage"] as string;
                ViewData["SuccessMessage"] = TempData["SuccessMessage"] as string;
            }
            // Pass the search string back to the view
            ViewData["BlogPage"] = blogPage;
            return View();
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
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