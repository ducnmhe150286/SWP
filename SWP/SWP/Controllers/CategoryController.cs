using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using System.Collections.Generic;

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
                using (var context = new SWPContext())
                {
                    // Lấy tên người dùng hiện tại, bạn cần cập nhật logic lấy tên người dùng theo cách của bạn
                    string currentUserName = "User1"; // Thay thế bằng logic lấy tên người dùng

                    // Chuyển đổi giá trị từ kiểu string sang kiểu int?
                    int? status = 1; // Status mặc định là 1

                    // Tạo mới Category
                    Category categoryToAdd = new Category
                    {
                        CategoryName = newCategory.CategoryName,
                        CreatedBy = currentUserName,
                        UpdateBy = currentUserName,
                        Status = status,
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
        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (var context = new SWPContext())
            {
                var categoryToDelete = context.Categories.Find(id);
                if (categoryToDelete != null)
                {
                    context.Categories.Remove(categoryToDelete);
                    context.SaveChanges();
                }
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
    

