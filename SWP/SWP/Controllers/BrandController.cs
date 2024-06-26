﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using System.Text.RegularExpressions;

namespace SWP.Controllers
{
    public class BrandController : Controller
    {

        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            // Lấy userId từ session
            var userId = HttpContext.Session.GetInt32("USER_ID");


            // Kiểm tra userId có null không
            if (userId == null)
            {
                // Nếu userId null, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }
            else
            {
                using (var context = new SWPContext())
                {
                    // Lấy RoleId của User dựa trên userId, chẳng hạn từ CSDL hoặc bất kỳ nguồn dữ liệu nào khác
                    var user = context.Users.Find(userId);

                    // Kiểm tra nếu RoleId của User là 2
                    if (user != null && user.RoleId == 2)
                    {
                        // Thực hiện các hành động nếu RoleId của User là 2
                        return RedirectToAction("Login", "Account");
                    }
                }
                using (var context = new SWPContext())
                {
                    var totalBrands = context.Brands.Count();

                    var brandList = context.Brands
                        .OrderBy(b => b.BrandId)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var totalPages = (int)Math.Ceiling((double)totalBrands / pageSize);

                    ViewBag.CurrentPage = page;
                    ViewBag.TotalPages = totalPages;

                    return View(brandList);
                }
            }
        }
        [HttpGet]
        public ActionResult AddBrand()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddBrand(Brand newBrand)
        {
            if (ModelState.IsValid)
            {
                if (!IsValidName(newBrand.BrandName))
                {
                    ModelState.AddModelError("BrandName", "Tên thương hiệu chỉ được chứa chữ cái.");
                    // Xử lý lỗi nếu cần thiết
                    return View(newBrand);
                }

                using (var context = new SWPContext())
                {
                    // Kiểm tra xem tên thương hiệu đã tồn tại chưa
                    if (context.Brands.Any(b => b.BrandName == newBrand.BrandName))
                    {
                        ModelState.AddModelError("BrandName", "Tên thương hiệu đã tồn tại. Vui lòng chọn tên khác.");
                        // Xử lý lỗi nếu cần thiết
                        return View(newBrand);
                    }
                    // Lấy tên người dùng hiện tại, bạn cần cập nhật logic lấy tên người dùng theo cách của bạn
                    string currentUserName = "User1"; // Thay thế bằng logic lấy tên người dùng

                    // Chuyển đổi giá trị từ kiểu string sang kiểu int?
                    int? status = 1; // Status mặc định là 1

                    // Tạo mới Brand
                    Brand brandToAdd = new Brand
                    {
                        BrandName = newBrand.BrandName,
                        CreatedBy = currentUserName,
                        UpdateBy = currentUserName,
                        Status = status,
                        CreatedDate = DateTime.Now
                    };

                    // Thêm mới Brand vào database
                    context.Brands.Add(brandToAdd);
                    context.SaveChanges();
                }

                // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại trang AddBrand với thông tin nhập trước đó
            return View(newBrand);
        }

        private bool IsValidName(string name)
        {
            // Kiểm tra xem tên chỉ chứa chữ cái
            return !string.IsNullOrWhiteSpace(name) && Regex.IsMatch(name, "^[a-zA-Zà-ỹẠ-Ỹ\\s]+$");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (var context = new SWPContext())
            {
                var brandToDelete = context.Brands.Include(b => b.Products).FirstOrDefault(b => b.BrandId == id);

                if (brandToDelete == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem có sản phẩm đang sử dụng Brand hay không
                if (brandToDelete.Products.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa vì thương hiệu này đang có sản phẩm sử dụng.";
                    return RedirectToAction(nameof(Index));
                }

                context.Brands.Remove(brandToDelete);
                context.SaveChanges();
            }

            // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (var context = new SWPContext())
            {
                var brandToEdit = context.Brands.Find(id);
                if (brandToEdit == null)
                {
                    // Xử lý khi không tìm thấy Category
                    return NotFound();
                }

                return View(brandToEdit);
            }
        }
        [HttpPost]
        public IActionResult Edit(Brand editedBrand)
        {
            if (ModelState.IsValid)
            {
                using (var context = new SWPContext())
                {
                    var existingBrand = context.Brands.Find(editedBrand.BrandId);

                    if (existingBrand != null)
                    {
                        // Kiểm tra xem có sản phẩm sử dụng Category không
                        if (HasProductsInBrand(existingBrand.BrandId))
                        {
                            // Nếu có sản phẩm sử dụng danh mục này, chỉ cho phép chỉnh sửa tên danh mục
                            existingBrand.BrandName = existingBrand.BrandName;
                        }
                        else
                        {
                            // Kiểm tra xem người dùng có chỉnh sửa cả trạng thái không
                            if (existingBrand.Status != existingBrand.Status)
                            {
                                // Nếu người dùng cố gắng chỉnh sửa cả trạng thái
                                TempData["ErrorMessage"] = "Không thể thay đổi trạng thái vì đang có sản phẩm sử dụng danh mục này.";
                                return RedirectToAction("Index");
                            }

                            // Kiểm tra xem tên mới của danh mục có trùng với bất kỳ tên nào khác trong cơ sở dữ liệu không
                            var BrandNameExists = context.Brands.Any(c => c.BrandId != existingBrand.BrandId && c.BrandName == editedBrand.BrandName);
                            if (BrandNameExists)
                            {
                                ModelState.AddModelError("CategoryName", "Tên Thương hiệu này đã tồn tại. Vui lòng chọn tên khác.");
                                return View(existingBrand);
                            }

                            // Cho phép chỉnh sửa cả tên và trạng thái
                            existingBrand.BrandName = existingBrand.BrandName;
                            existingBrand.Status = existingBrand.Status;
                        }

                        context.SaveChanges();
                    }
                    else
                    {
                        // Xử lý khi không tìm thấy Brand
                        return NotFound();
                    }
                }

                // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
                TempData["SuccessMessage"] = "Brand đã được cập nhật thành công.";
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại trang Edit với thông tin nhập trước
            return View(editedBrand);
        }

        private bool HasProductsInBrand(int brandId)
        {
            using (var context = new SWPContext())
            {
                // Kiểm tra xem có sản phẩm nào sử dụng Brand có ID là brandId không
                var productsInBrand = context.Products.Where(p => p.BrandId == brandId).ToList();
                return productsInBrand.Any();
            }
        }
    }
}