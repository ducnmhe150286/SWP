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
		[Route("products/list")]
        public IActionResult Products(string searchString, int? statusFilter, int? categoryFilter, int? brandFilter)
        {
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
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                ViewBag.Products = productList;
                ViewData["Products"] = productList;
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

        [HttpPost]
        public IActionResult NewProduct(Product newProduct)
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
                }

                return RedirectToAction("Products");

            } catch(Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tạo mới sản phẩm: ";
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
        public IActionResult UpdateProduct(Product updatedProduct)
        {
            try
            {
                using (var context = new SWPContext())
                {
                    var existingProduct = context.Products.Find(updatedProduct.ProductId);
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
                var productToDelete = context.Products.Find(id);
                if (productToDelete != null)
                {
                    context.Products.Remove(productToDelete);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("Products");
        }
    }

}
