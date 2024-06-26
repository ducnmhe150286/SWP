﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SWP.Controllers
{

    public class CategoryController : Controller
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
                    var totalCategories = context.Categories.Count();

                    var categoryList = context.Categories
                        .OrderBy(c => c.CategoryId)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var totalPages = (int)Math.Ceiling((double)totalCategories / pageSize);

                    ViewBag.CurrentPage = page;
                    ViewBag.TotalPages = totalPages;

                    return View(categoryList);
                }
            }
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCategory(Category newCategory)
        {
            if (ModelState.IsValid)
            {
                if (!IsValidName(newCategory.CategoryName))
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục chỉ được chứa chữ cái.");
                    // Xử lý lỗi nếu cần thiết
                    return View(newCategory);
                }

                using (var context = new SWPContext())
                {
                    if (context.Categories.Any(c => c.CategoryName == newCategory.CategoryName))
                    {
                        ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại. Vui lòng chọn tên khác.");
                        // Xử lý lỗi nếu cần thiết
                        return View(newCategory);
                    }
                    // Lấy tên người dùng hiện tại, bạn cần cập nhật logic lấy tên người dùng theo cách của bạn
                    string currentUserName = "User1"; // Thay thế bằng logic lấy tên người dùng

                    // Chuyển đổi giá trị từ kiểu string sang kiểu int?
                    newCategory.Status = 1;

                    // Tạo mới Category
                    Category categoryToAdd = new Category
                    {
                        CategoryName = newCategory.CategoryName,
                        CreatedBy = currentUserName,
                        UpdateBy = currentUserName,
                        Status = newCategory.Status,
                        CreatedDate = DateTime.Now
                    };

                    // Thêm mới Category vào database
                    context.Categories.Add(categoryToAdd);
                    context.SaveChanges();
                }

                // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại trang AddCategory với thông tin nhập trước đó
            return View(newCategory);
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
                var categoryToDelete = context.Categories.Include(c => c.Products).FirstOrDefault(c => c.CategoryId == id);

                if (categoryToDelete == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem có sản phẩm đang sử dụng Category hay không
                if (categoryToDelete.Products.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa vì thương hiệu đang có sản phẩm sử dụng.";
                    return RedirectToAction(nameof(Index));
                }

                context.Categories.Remove(categoryToDelete);
                context.SaveChanges();
            }

            // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            using (var context = new SWPContext())
            {
                var categoryToEdit = context.Categories.Find(id);
                if (categoryToEdit == null)
                {
                    // Xử lý khi không tìm thấy Category
                    return NotFound();
                }

                return View(categoryToEdit);
            }
        }

        [HttpPost]
        public IActionResult Edit(Category editedCategory)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tính hợp lệ của tên danh mục
                if (!IsValidName(editedCategory.CategoryName))
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục chỉ được chứa chữ cái.");
                    return View(editedCategory);
                }

                using (var context = new SWPContext())
                {
                    var existingCategory = context.Categories.Find(editedCategory.CategoryId);

                    if (existingCategory != null)
                    {
                        // Kiểm tra xem có sản phẩm sử dụng Category không
                        if (HasProductsInCategory(existingCategory.CategoryId))
                        {
                            // Nếu có sản phẩm sử dụng danh mục này, chỉ cho phép chỉnh sửa tên danh mục
                            existingCategory.CategoryName = editedCategory.CategoryName;
                        }
                        else
                        {
                            // Kiểm tra xem người dùng có chỉnh sửa cả trạng thái không
                            if (editedCategory.Status != existingCategory.Status)
                            {
                                // Nếu người dùng cố gắng chỉnh sửa cả trạng thái
                                TempData["ErrorMessage"] = "Không thể thay đổi trạng thái vì đang có sản phẩm sử dụng danh mục này.";
                                return RedirectToAction("Index");
                            }

                            // Kiểm tra xem tên mới của danh mục có trùng với bất kỳ tên nào khác trong cơ sở dữ liệu không
                            var categoryNameExists = context.Categories.Any(c => c.CategoryId != editedCategory.CategoryId && c.CategoryName == editedCategory.CategoryName);
                            if (categoryNameExists)
                            {
                                ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại. Vui lòng chọn tên khác.");
                                return View(editedCategory);
                            }

                            // Cho phép chỉnh sửa cả tên và trạng thái
                            existingCategory.CategoryName = editedCategory.CategoryName;
                            existingCategory.Status = editedCategory.Status;
                        }

                        context.SaveChanges();
                    }
                    else
                    {
                        // Xử lý khi không tìm thấy Category
                        return NotFound();
                    }
                }

                // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
                TempData["SuccessMessage"] = "Category đã được cập nhật thành công.";
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại trang Edit với thông tin nhập trước
            return View(editedCategory);
        }   
        private bool HasProductsInCategory(int categoryId)
        {
            using (var context = new SWPContext())
            {
                // Kiểm tra xem có sản phẩm nào sử dụng Category có ID là categoryId không
                var productsInCategory = context.Products.Where(p => p.CategoryId == categoryId).ToList();
                return productsInCategory.Any();
            }
        }
    }
}
