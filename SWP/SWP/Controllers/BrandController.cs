using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using System.Text.RegularExpressions;

namespace SWP.Controllers
{
    public class BrandController : Controller
    {
        public IActionResult Index()
        {
            using (var context = new SWPContext())
            {
                var brandList = context.Brands.ToList();

                ViewBag.Brand = brandList;
                ViewData["Brands"] = brandList;

                return View(brandList);
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
                    ModelState.AddModelError(string.Empty, "Không thể xóa Category vì có sản phẩm đang sử dụng.");
                    return RedirectToAction("Index"); // Hoặc chuyển hướng đến trang nào đó để thông báo lỗi.
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
                        existingBrand.BrandName = editedBrand.BrandName;
                        existingBrand.Status = editedBrand.Status;

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
            return View(editedBrand);
        }
    }
}
