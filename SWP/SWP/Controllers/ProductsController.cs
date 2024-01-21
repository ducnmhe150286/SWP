using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Dao;
using SWP.Models;
using System.Collections.Generic;
using System.Linq;

namespace SWP.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [Route("products/list")]
        public IActionResult Products(string searchString, int? statusFilter, int? categoryFilter, int? brandFilter)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            using (var context = new SWPContext())
            {
                var brands = context.Brands.ToList();
                var categories = context.Categories.ToList();
                var productList = context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Where(p =>
                    (string.IsNullOrEmpty(searchString) ||
                    p.ProductName.Contains(searchString) ||
                    p.Brand.BrandName.Contains(searchString) ||
                    p.Category.CategoryName.Contains(searchString)) &&
                    (!statusFilter.HasValue || p.Status == statusFilter) &&
                    (!categoryFilter.HasValue || p.CategoryId == categoryFilter) &&
                    (!brandFilter.HasValue || p.BrandId == brandFilter)
                )
                .ToList();
                //foreach (var product in productList)
                //{
                //    foreach (var productImage in product.ProductImages)
                //    {
                //        productImage.Path = Path.Combine(imagesFolder, productImage.Path);
                //    }
                //}
                foreach (var product in productList)
                {
                    foreach (var productImage in product.ProductImages)
                    {
                        //productImage.Path = Path.Combine("/Images/tap-boxing-tay-co-to-khong-1.jpg");
                        productImage.Path = Path.Combine(productImage.Path);

                    }
                }
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                ViewBag.Products = productList;
                ViewData["SearchString"] = searchString;
                ViewBag.StatusFilter = statusFilter;
                ViewBag.CategoryFilter = categoryFilter;
                ViewBag.BrandFilter = brandFilter;


            }
            return View();
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
                var brandList = context.Brands.ToList();
                var categoryList = context.Categories.ToList();

                ViewBag.Brand = brandList;
                ViewBag.Category = categoryList;

            }
            return View();
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


        [HttpPost]
        public IActionResult NewProduct(Product newProduct, List<IFormFile> imageFiles)
        {
            try
            {
                newProduct.CreatedDate = DateTime.Now;
                newProduct.CreatedBy = null;
                newProduct.UpdateBy = null;
                newProduct.ProductImages = null;
                using (var context = new SWPContext())
                {
                    newProduct.ProductName = string.IsNullOrWhiteSpace(newProduct.ProductName) ? null : newProduct.ProductName;
                    newProduct.Description = string.IsNullOrWhiteSpace(newProduct.Description) ? null : newProduct.Description;
                    newProduct.Price = newProduct.Price.HasValue ? newProduct.Price : null;
                    newProduct.Quantity = newProduct.Quantity.HasValue ? newProduct.Quantity : null;
                    newProduct.IsSale = newProduct.IsSale.HasValue ? newProduct.IsSale : false;
                    newProduct.Status = newProduct.Status.HasValue ? newProduct.Status : null;
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
                            string uniqueFileName = Path.GetFileName(file.FileName);

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
                }
                return RedirectToAction("Products");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tạo mới sản phẩm: " + ex;
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
                    .FirstOrDefault(p => p.ProductId == id);

                var brands = context.Brands.ToList();
                var categories = context.Categories.ToList();
                ViewBag.Product = product;
                ViewBag.Brands = brands;
                ViewBag.Categories = categories;
            }
            return View();
        }



        [HttpPost]
        public IActionResult UpdateProduct(Product updatedProduct, List<IFormFile> imageFiles)
        {
            try
            {
                using (var context = new SWPContext())
                {
                    var existingProduct = context.Products.Find(updatedProduct.ProductId);

                    var existingProductImage = context.Products
                        .Include(p => p.ProductImages)
                        .FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);

                    if (existingProduct != null)
                    {

                        existingProduct.ProductName = updatedProduct.ProductName;
                        existingProduct.Description = updatedProduct.Description;
                        existingProduct.Price = updatedProduct.Price;
                        existingProduct.Quantity = updatedProduct.Quantity;
                        existingProduct.IsSale = updatedProduct.IsSale;
                        existingProduct.Status = updatedProduct.Status;
                        existingProduct.BrandId = updatedProduct.BrandId;
                        existingProduct.CategoryId = updatedProduct.CategoryId;
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

                        context.SaveChanges();
                    }
                }

                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi cập nhật sản phẩm: " + ex.Message;
                return RedirectToAction("EditProduct", new { id = updatedProduct.ProductId });
            }
        }


        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            using (var context = new SWPContext())
            {
                var productToDelete = context.Products
                    .Include(p => p.ProductImages) // Nạp danh sách ProductImages
                    .FirstOrDefault(p => p.ProductId == id);

                if (productToDelete != null)
                {
                    // Xóa tất cả ProductImages liên quan
                    foreach (var productImage in productToDelete.ProductImages)
                    {
                        // Xóa ảnh từ thư mục
                        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", productImage.Path);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }

                    // Xóa tất cả ProductImages từ cơ sở dữ liệu
                    context.ProductImages.RemoveRange(productToDelete.ProductImages);

                    // Xóa sản phẩm chính
                    context.Products.Remove(productToDelete);

                    context.SaveChanges();
                }
            }

            return RedirectToAction("Products");
        }


    }
}
