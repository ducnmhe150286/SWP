using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using System.Text.RegularExpressions;

namespace SWP.Controllers
{
    public class SizeController : Controller
    {
        public IActionResult Index()
        {
            using (var context = new SWPContext())
            {
                var sizeList = context.Sizes.ToList();

                ViewBag.Size = sizeList;
                ViewData["Sizes"] = sizeList;

                return View(sizeList);
            }
        }
        [HttpGet]
        public ActionResult AddSize()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddSize(Size newSize)
        {
            if (ModelState.IsValid)
            {
                if (!IsValidName(newSize.SizeName))
                {
                    ModelState.AddModelError("SizeName", "Tên danh mục chỉ được chứa chữ cái.");
                    // Xử lý lỗi nếu cần thiết
                    return View(newSize);
                }

                using (var context = new SWPContext())
                {
                    if (context.Sizes.Any(s => s.SizeName == newSize.SizeName))
                    {
                        ModelState.AddModelError("SizeName", "Tên danh mục đã tồn tại. Vui lòng chọn tên khác.");
                        // Xử lý lỗi nếu cần thiết
                        return View(newSize);
                    }
                    // Lấy tên người dùng hiện tại, bạn cần cập nhật logic lấy tên người dùng theo cách của bạn
                    string currentUserName = "User1"; // Thay thế bằng logic lấy tên người dùng

                    // Chuyển đổi giá trị từ kiểu string sang kiểu int?
                    newSize.Status = 1;

                    // Tạo mới Category
                    Size sizeToAdd = new Size
                    {
                        SizeName = newSize.SizeName,
                        CreatedBy = currentUserName,
                        UpdateBy = currentUserName,
                        Status = newSize.Status,
                        CreatedDate = DateTime.Now
                    };

                    // Thêm mới Category vào database
                    context.Sizes.Add(sizeToAdd);
                    context.SaveChanges();
                }

                // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại trang AddCategory với thông tin nhập trước đó
            return View(newSize);
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
                var sizeToDelete = context.Sizes.Include(s => s.ProductDetails).FirstOrDefault(s => s.SizeId == id);

                if (sizeToDelete == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem có sản phẩm đang sử dụng Category hay không
                if (sizeToDelete.ProductDetails.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa vì thương hiệu đang có sản phẩm sử dụng.";
                    return RedirectToAction(nameof(Index));
                }

                context.Sizes.Remove(sizeToDelete);
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
                var sizeToEdit = context.Sizes.Find(id);
                if (sizeToEdit == null)
                {
                    // Xử lý khi không tìm thấy Category
                    return NotFound();
                }

                return View(sizeToEdit);
            }
        }

        [HttpPost]
        public IActionResult Edit(Size editedSize)
        {
            if (ModelState.IsValid)
            {
                using (var context = new SWPContext())
                {
                    var existingSize = context.Sizes.Find(editedSize.SizeId);

                    if (existingSize != null)
                    {

                        existingSize.SizeName = editedSize.SizeName;
                        existingSize.Status = editedSize.Status;

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
            return View(editedSize);
        }
    }
}
