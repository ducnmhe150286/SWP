using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWP.Dao;
using SWP.Models;
using SWP.Utilities;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SWP.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        //public ManageUsersDao usersManage;
        // public ProductsController()
        // {
        //     usersManage = new ManageUsersDao();
        // }
        [Route("products/list")]
        public IActionResult Products(string searchString, int? statusFilter, int? categoryFilter, int? brandFilter, int page = 1)
        {
            int pageSize = 5;
            string webRootPath = _webHostEnvironment.WebRootPath;
            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            using (var context = new SWPContext())
            {
                var brands = context.Brands.Where(b => b.Status == 1).ToList();
                var categories = context.Categories.Where(c => c.Status == 1).ToList();

                var query = context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.ProductDetails)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductDetails)
                    .ThenInclude(pd => pd.Color)
                    .Include(p => p.ProductDetails)
                    .ThenInclude(pd => pd.Size)
                .Where(p =>
    (!statusFilter.HasValue ||
        ((statusFilter != 2 && p.Status == statusFilter) ||
           (statusFilter == 4 && (p.ProductDetails == null || !p.ProductDetails.Any())) ||
            (statusFilter == 2 && p.ProductDetails != null && p.ProductDetails.Any() && p.ProductDetails.All(pd => pd.Quantity == 0))
        )
    ) &&
    (!categoryFilter.HasValue || p.CategoryId == categoryFilter) &&
    (!brandFilter.HasValue || p.BrandId == brandFilter)
);







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

                var paginatedList = PaginatedList<Product>.Create(productList.AsQueryable(), page, pageSize);

                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                ViewBag.Products = paginatedList;
                ViewData["SearchString"] = searchString;
                ViewBag.StatusFilter = statusFilter;
                ViewBag.CategoryFilter = categoryFilter;
                ViewBag.BrandFilter = brandFilter;
                ViewData["ErrorMessage"] = TempData["ErrorMessage"] as string;
                ViewData["SuccessMessage"] = TempData["SuccessMessage"] as string;
            }
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

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            using (var context = new SWPContext())
            {
                var brandList = context.Brands.Where(b => b.Status == 1).ToList();
                var categoryList = context.Categories.Where(c => c.Status == 1).ToList();
                var colorList = context.Colors.Where(c => c.Status == 1).ToList();
                var sizeList = context.Sizes.Where(s => s.Status == 1).ToList();

                ViewBag.Brand = brandList;
                ViewBag.Color = colorList;
                ViewBag.Size = sizeList;
                ViewBag.Category = categoryList;
                ViewData["ErrorMessage"] = TempData["ErrorMessage"] as string;


            }
            return View();
        }
        public IActionResult DetailProduct(int id)
        {
            using (var context = new SWPContext())
            {
                var product = context.Products
                    .Include(p => p.ProductDetails)
                    .ThenInclude(pd => pd.Color)
                    .Include(p => p.ProductDetails)
                    .ThenInclude(pd => pd.Size)
                    .FirstOrDefault(p => p.ProductId == id);

                var allProductDetails = product?.ProductDetails.ToList();
                var colorList = context.Colors.Where(c => c.Status == 1).ToList();
                var sizeList = context.Sizes.Where(s => s.Status == 1).ToList();

                ViewBag.AllProductDetails = allProductDetails;
                ViewBag.Colors = colorList;
                ViewBag.Sizes = sizeList;
                ViewBag.Product = product;
                ViewData["ErrorMessage"] = TempData["ErrorMessage"] as string;
                return View();
            }
        }


        public IActionResult GetImage(string imageName)
        {
            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            string imagePath = Path.Combine(imagesFolder, imageName);

            if (System.IO.File.Exists(imagePath))
            {
                byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
                return File(imageData, "image/jpeg");
            }
            else
            {
                return NotFound();
            }
        }
        private string GetUniqueFileName(string fileName)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string uniqueFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + timestamp + Path.GetExtension(fileName);
            return uniqueFileName;
        }


        [HttpPost]
        public IActionResult NewProduct(Product newProduct,int? ColorId,int? SizeId ,string Feature, string Attributes, List<IFormFile> imageFiles,string Price)
        {
            try
            {
                newProduct.CreatedDate = DateTime.Now;
                newProduct.CreatedBy = null;
                newProduct.UpdateBy = null;
                //newProduct.ProductImages = null;
               
                using (var context = new SWPContext())
                {
                    if (context.Products.Any(p => p.ProductName == newProduct.ProductName &&
                                            p.BrandId == newProduct.BrandId &&
                                            p.CategoryId == newProduct.CategoryId))
                    {
                        TempData["ErrorMessage"] = " Tên sản phẩm đã tồn tại trong cùng một thương hiệu và danh mục.";
                        return RedirectToAction("CreateProduct");
                    }
                    newProduct.ProductName = string.IsNullOrWhiteSpace(newProduct.ProductName) ? null : newProduct.ProductName;
                    newProduct.Description = string.IsNullOrWhiteSpace(newProduct.Description) ? null : newProduct.Description;
                    newProduct.Price = decimal.Parse(Price);
                    //newProduct.Quantity = newProduct.Quantity.HasValue ? newProduct.Quantity : null;
                    //newProduct.IsSale = newProduct.IsSale.HasValue ? newProduct.IsSale : false;
                    newProduct.Status = newProduct.Status.HasValue ? newProduct.Status : 0;
                    newProduct.BrandId = newProduct.BrandId;
                    newProduct.CategoryId = newProduct.CategoryId;
                    context.Products.Add(newProduct);
                    context.SaveChanges();
                    string wwwrootFolder = _webHostEnvironment.WebRootPath;
                    string imagesFolder = Path.Combine(wwwrootFolder, "Images");

                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    foreach (var file in imageFiles)
                    {
                        if (file.Length > 0)
                        {
                            // Tạo một tên tệp duy nhất để tránh xung đột
                            string uniqueFileName = GetUniqueFileName(file.FileName);

                            // Xây dựng đường dẫn đầy đủ để lưu ảnh trong thư mục Images
                            string imagePath = Path.Combine(imagesFolder, uniqueFileName);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            // Chỉ lưu tên tệp trong cột Path của cơ sở dữ liệu
                            var productImage = new ProductImage
                            {
                                ProductId = newProduct.ProductId,
                                Path = uniqueFileName, // Chỉ lưu tên tệp
                                CreateDate = DateTime.Now,
                                CreatedBy = null,
                                UpdateBy = null,
                                Status = 1,
                            };
                            context.ProductImages.Add(productImage);
                        }
                    }

                    context.SaveChanges();
                    //if (ColorId.HasValue || SizeId.HasValue || !string.IsNullOrWhiteSpace(Feature) || !string.IsNullOrWhiteSpace(Attributes))
                    //{
                    //    int generatedProductId = newProduct.ProductId;
                    //    var productDetailEntity = new ProductDetail
                    //    {
                    //        ProductId = generatedProductId,
                    //        ColorId = ColorId.HasValue ? ColorId : null,
                    //        SizeId = SizeId.HasValue ? SizeId : null,
                    //        Feature = Feature,
                    //        Attributes = Attributes,
                    //        CreatedDate = DateTime.Now,
                    //        CreatedBy = null,
                    //        UpdateBy = null,
                    //        Status = 1,
                    //    };
                    //    context.ProductDetails.Add(productDetailEntity);
                    //    context.SaveChanges();
                    //}
                }
                TempData["SuccessMessage"] = "Tạo mới sản phẩm thành công!";
                return RedirectToAction("Products");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo mới sản phẩm: " ;
                return RedirectToAction("CreateProduct");
            }
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            using (var context = new SWPContext())
            {
                var product = context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductDetails)
                    .FirstOrDefault(p => p.ProductId == id);

                var firstProductDetail = product?.ProductDetails?.FirstOrDefault();
                var brands = context.Brands.Where(b => b.Status == 1).ToList();
                var categories = context.Categories.Where(c => c.Status == 1).ToList();
                var colorList = context.Colors.Where(c => c.Status == 1).ToList();
                var sizeList = context.Sizes.Where(s => s.Status == 1).ToList();

                ViewBag.ProductDetail = firstProductDetail;
                ViewBag.Color = colorList;
                ViewBag.Size = sizeList;
                ViewBag.Product = product;
                ViewBag.Brands = brands;
                ViewBag.Categories = categories;
                ViewData["ErrorMessage"] = TempData["ErrorMessage"] as string;

            }
            return View();
        }



        [HttpPost]
        public IActionResult UpdateProduct(Product updatedProduct, int? ColorId, int? SizeId, string Feature, string Attributes, List<IFormFile> imageFiles,string Price)
        {
            try
            {
                using (var context = new SWPContext())
                {
                    // Kiểm tra xem tên sản phẩm đã được cập nhật có tồn tại cho một sản phẩm khác không
                    if (context.Products.Any(p => p.ProductName == updatedProduct.ProductName &&
                                            p.BrandId == updatedProduct.BrandId &&
                                            p.CategoryId == updatedProduct.CategoryId &&
                                            p.ProductId != updatedProduct.ProductId))
                    {
                        TempData["ErrorMessage"] = "Tên sản phẩm đã tồn tại trong cùng một danh mục và thương hiệu.";
                        return RedirectToAction("EditProduct", new { id = updatedProduct.ProductId });
                    }

                    var existingProduct = context.Products
                        .Include(p => p.ProductDetails) 
                        .FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);
                    if (existingProduct.Status == 0 && existingProduct.ProductDetails.Any() == false && updatedProduct.Status == 1)
                    {
                        TempData["ErrorMessage"] = "Không thể cập nhật trạng thái sản phẩm do không có thông tin chi tiết về sản phẩm.";
                        return RedirectToAction("EditProduct", new { id = updatedProduct.ProductId });
                    }


                    var existingProductImage = context.Products
                        .Include(p => p.ProductImages)
                        .FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);

                    if (existingProduct != null)
                    {
                        existingProduct.ProductName = updatedProduct.ProductName;
                        existingProduct.Description = updatedProduct.Description;
                        existingProduct.Price = decimal.Parse(Price);
                        existingProduct.IsSale = updatedProduct.IsSale;
                        existingProduct.Status = updatedProduct.Status;
                        existingProduct.BrandId = updatedProduct.BrandId;
                        existingProduct.CategoryId = updatedProduct.CategoryId;
                        existingProduct.Feature = updatedProduct.Feature;
                        existingProduct.Attributes = updatedProduct.Attributes;

                        // Kiểm tra có tệp tin mới được chọn hay không
                        if (imageFiles != null && imageFiles.Count > 0)
                        {
                            // Xóa toàn bộ ảnh cũ
                            foreach (var oldImage in existingProductImage.ProductImages)
                            {
                                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", oldImage.Path);
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }
                            }

                            // Xóa toàn bộ ảnh cũ từ cơ sở dữ liệu
                            existingProductImage.ProductImages.Clear();

                            // Thêm ảnh mới
                            foreach (var file in imageFiles)
                            {
                                if (file.Length > 0)
                                {
                                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", uniqueFileName);

                                    using (var stream = new FileStream(imagePath, FileMode.Create))
                                    {
                                        file.CopyTo(stream);
                                    }

                                    // Lưu thông tin ảnh vào cơ sở dữ liệu
                                    var productImage = new ProductImage
                                    {
                                        Path = uniqueFileName,
                                        CreateDate = DateTime.Now,
                                        CreatedBy = null,
                                        UpdateBy = null,
                                        Status = 1,
                                    };

                                    existingProductImage.ProductImages.Add(productImage);
                                }
                            }
                        }

                        context.SaveChanges();
                    }
                }

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật sản phẩm: " + ex.Message;
                return RedirectToAction("EditProduct", new { id = updatedProduct.ProductId });
            }
        }

        [HttpPost]

        public IActionResult UpdateDetailProduct(ProductDetail updatedDetail, List<int> ColorId, List<int> SizeId, List<int> Quanties)
        {
            try
            {
                using (var context = new SWPContext())
                {
                    var groupedData = ColorId
                        .Zip(SizeId, (color, size) => new { ColorId = color, SizeId = size })
                        .Zip(Quanties, (pair, quantity) => new { ColorId = pair.ColorId, SizeId = pair.SizeId, Quantity = quantity })
                        .GroupBy(group => new { group.ColorId, group.SizeId })
                        .ToList();

                    foreach (var group in groupedData)
                    {
                        var totalQuantity = group.Sum(g => g.Quantity);

                        var existingDetail = context.ProductDetails
                            .FirstOrDefault(pd => pd.ProductId == updatedDetail.ProductId && pd.ColorId == group.Key.ColorId && pd.SizeId == group.Key.SizeId);

                        if (existingDetail != null)
                        {
                            // Update existing detail
                            existingDetail.Quantity = totalQuantity;
                            existingDetail.UpdateBy = null; // Set the update user as needed
                            existingDetail.Status = 1; // Set the status as needed
                        }
                        else
                        {
                            // Create new detail
                            var newProductDetail = new ProductDetail
                            {
                                ProductId = updatedDetail.ProductId,
                                ColorId = group.Key.ColorId,
                                SizeId = group.Key.SizeId,
                                Quantity = totalQuantity,
                                CreatedDate = DateTime.Now,
                                CreatedBy = null, // Set the creator user as needed
                                UpdateBy = null, // Set the update user as needed
                                Status = 1, // Set the status as needed
                            };

                            context.ProductDetails.Add(newProductDetail);
                        }
                    }

                    context.SaveChanges();
                }

                TempData["SuccessMessage"] = "Nhập sản phẩm thành công!";

                // Redirect to the appropriate action or view
                // For example, you might redirect to the product details view
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi nhập sản phẩm: " + ex.Message;

                // Redirect to the appropriate action or view
                // For example, you might redirect to the product details view
                return RedirectToAction("DetailProduct", new { id = updatedDetail.ProductId });
            }
        }



        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            using (var context = new SWPContext())
            {
                var productToDelete = context.ProductDetails
                    .Include(pd => pd.Product.ProductImages)
                    .Where(pd => pd.ProductId == id)
                    .FirstOrDefault();

                if (productToDelete != null && productToDelete.Quantity == 0)
                {
                    // Xóa tất cả ProductImages liên quan
                    foreach (var productImage in productToDelete.Product.ProductImages)
                    {
                        // Xóa ảnh từ thư mục
                        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", productImage.Path);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }

                    // Xóa tất cả ProductImages từ cơ sở dữ liệu
                    context.ProductImages.RemoveRange(productToDelete.Product.ProductImages);

                    // Xóa sản phẩm chính
                    context.ProductDetails.Remove(productToDelete);

                    context.SaveChanges();
                    TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa sản phẩm. Số lượng sản phẩm lớn hơn 0.";
                }
            }

            return RedirectToAction("Products");
        }
        public IActionResult UpdateStatusProduct(int id, string searchString, int? statusFilter, int? categoryFilter, int? brandFilter, int page = 1)
        {
            using (var context = new SWPContext())
            {
                var productToUpdate = context.Products
                    .Include(p => p.ProductDetails) // Include ProductDetail for checking its existence
                    .Where(p => p.ProductId == id)
                    .FirstOrDefault();

                if (productToUpdate != null)
                {
                    // Check if status is currently 0 and productDetail exists
                    if (productToUpdate.Status == 0 && productToUpdate.ProductDetails.Any() == false)
                    {
                        // Set status to the opposite value

                        TempData["ErrorMessage"] = "Không thể cập nhật trạng thái sản phẩm do không có thông tin nhập chi tiết về sản phẩm.";

                    }
                    else
                    {
                        productToUpdate.Status = (productToUpdate.Status == 1) ? 0 : 1;
                        context.SaveChanges();
                        TempData["SuccessMessage"] = "Cập nhật trạng thái sản phẩm thành công!";

                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                }
            }

            // Include search and pagination parameters when redirecting
            return RedirectToAction("Products", new
            {
                searchString = searchString,
                statusFilter = statusFilter,
                categoryFilter = categoryFilter,
                brandFilter = brandFilter,
                page = page
            });
        }



    }
}