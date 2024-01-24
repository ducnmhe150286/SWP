using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SWP.Controllers
{

    public class CategoryController : Controller
    {

        public IActionResult Index()
        {
            using (var context = new SWPContext())
            {
                var categoryList = context.Categories.ToList();

                ViewBag.Category = categoryList;
                ViewData["Categories"] = categoryList;

                return View(categoryList);
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
                    return RedirectToAction("Index"); // Hoặc chuyển hướng đến trang nào đó để thông báo lỗi.
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
                using (var context = new SWPContext())
                {
                    var existingCategory = context.Categories.Find(editedCategory.CategoryId);

                    if (existingCategory != null)
                    {
                        existingCategory.CategoryName = editedCategory.CategoryName;
                        existingCategory.Status = editedCategory.Status;

                        context.SaveChanges();
                    }
                    else
                    {
                        // Xử lý khi không tìm thấy Category
                        return NotFound();
                    }
                }

                // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại trang Edit với thông tin nhập trước
            return View(editedCategory);
        }
    }
}
    

