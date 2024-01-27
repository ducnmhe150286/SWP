﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWP.Dao;
using SWP.Models;
using SWP.Utilities;
using System.Collections.Generic;
using System.Drawing;
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
                //string email = HttpContext.Session.GetString("USER_EMAIL");

                //if (usersManage.CheckAdmin(email) == false)
                //{
                //    return RedirectToAction("Error");
                //}
                var brands = context.Brands.ToList();
                var categories = context.Categories.ToList();
                var productList = context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductDetails)
                    .ThenInclude(pd => pd.Color)
                    .Include(p => p.ProductDetails)
                    .ThenInclude(pd => pd.Size)
                    .Where(p =>
                    (string.IsNullOrEmpty(searchString) ||
                    p.ProductName.Contains(searchString) ||
                    p.Brand.BrandName.Contains(searchString) ||
                    p.Category.CategoryName.Contains(searchString)) && 
                    (!statusFilter.HasValue ||
                        (
                            (statusFilter != 2 && statusFilter != 1) ? p.Status == statusFilter :
                            (statusFilter == 1 ? (p.Status == 1 && p.Quantity > 0) : p.Quantity == 0)
                        )
                    ) &&
                    (!categoryFilter.HasValue || p.CategoryId == categoryFilter) &&
                    (!brandFilter.HasValue || p.BrandId == brandFilter)
                )
                .OrderByDescending(p => p.CreatedDate)
                .ToList();
                var paginatedList = PaginatedList<Product>.Create(productList.AsQueryable(), page, pageSize);

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
                var colorList = context.Colors.ToList();
                var sizeList = context.Sizes.ToList();

                ViewBag.Brand = brandList;
                ViewBag.Color = colorList;
                ViewBag.Size = sizeList;
                ViewBag.Category = categoryList;
                ViewData["ErrorMessage"] = TempData["ErrorMessage"] as string;


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
        private string GetUniqueFileName(string fileName)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string uniqueFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + timestamp + Path.GetExtension(fileName);
            return uniqueFileName;
        }


        [HttpPost]
        public IActionResult NewProduct(Product newProduct,int? ColorId,int? SizeId ,string Feature, string Attributes, List<IFormFile> imageFiles)
        {
            try
            {
                newProduct.CreatedDate = DateTime.Now;
                newProduct.CreatedBy = null;
                newProduct.UpdateBy = null;
                newProduct.ProductImages = null;
               
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
                    if (ColorId.HasValue || SizeId.HasValue || !string.IsNullOrWhiteSpace(Feature) || !string.IsNullOrWhiteSpace(Attributes))
                    {
                        int generatedProductId = newProduct.ProductId;
                        var productDetailEntity = new ProductDetail
                        {
                            ProductId = generatedProductId,
                            ColorId = ColorId.HasValue ? ColorId : null,
                            SizeId = SizeId.HasValue ? SizeId : null,
                            Feature = Feature,
                            Attributes = Attributes,
                            CreatedDate = DateTime.Now,
                            CreatedBy = null,
                            UpdateBy = null,
                            Status = 1,
                        };
                        context.ProductDetails.Add(productDetailEntity);
                        context.SaveChanges();
                    }
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
                var brands = context.Brands.ToList();
                var categories = context.Categories.ToList();
                var colorList = context.Colors.ToList();
                var sizeList = context.Sizes.ToList();
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
        public IActionResult UpdateProduct(Product updatedProduct, int? ColorId, int? SizeId, string Feature, string Attributes, List<IFormFile> imageFiles)
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

                        // Kiểm tra xem có tệp tin mới được chọn hay không
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
                        var productDetailEntity = context.ProductDetails.FirstOrDefault(pd => pd.ProductId == updatedProduct.ProductId);
                        if (productDetailEntity != null)
                        {
                            productDetailEntity.ColorId = ColorId;
                            productDetailEntity.SizeId = SizeId;
                            productDetailEntity.Feature = Feature;
                            productDetailEntity.Attributes = Attributes;
                            productDetailEntity.UpdateBy = null;
                            productDetailEntity.Status = 1;
                        }
                        else
                        {
                            // Nếu chi tiết sản phẩm chưa tồn tại, tạo mới
                            var newProductDetail = new ProductDetail
                            {
                                ProductId = updatedProduct.ProductId,
                                ColorId = ColorId,
                                SizeId = SizeId,
                                Feature = Feature,
                                Attributes = Attributes,
                                CreatedDate = DateTime.Now,
                                CreatedBy = null,
                                UpdateBy = null,
                                Status = 1,
                            };
                            context.ProductDetails.Add(newProductDetail);
                        }


                        context.SaveChanges();
                    }
                }
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";

                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật sản phẩm: ";

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
            TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";

            return RedirectToAction("Products");
        }


    }
}