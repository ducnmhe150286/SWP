using Microsoft.AspNetCore.Mvc;
using SWP.Models;

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
                using (var context = new SWPContext())
                {
                    // Lấy tên người dùng hiện tại, bạn cần cập nhật logic lấy tên người dùng theo cách của bạn
                    string currentUserName = "User1"; // Thay thế bằng logic lấy tên người dùng

                    // Chuyển đổi giá trị từ kiểu string sang kiểu int?
                    int? status = 1; // Status mặc định là 1

                    // Tạo mới Category
                    Brand brandToAdd = new Brand
                    {
                        BrandName = newBrand.BrandName,
                        CreatedBy = currentUserName,
                        UpdateBy = currentUserName,
                        Status = status,
                        CreatedDate = DateTime.Now
                    };

                    // Thêm mới Category vào database
                    context.Brands.Add(brandToAdd);
                    context.SaveChanges();
                }

                // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại trang AddCategory với thông tin nhập trước đó
            return View(newBrand);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (var context = new SWPContext())
            {
                var brandToDelete = context.Brands.Find(id);
                if (brandToDelete != null)
                {
                    context.Brands.Remove(brandToDelete);
                    context.SaveChanges();
                }
                // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
                return RedirectToAction("Index");
            }
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
