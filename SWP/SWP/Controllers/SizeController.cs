using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using System.Text.RegularExpressions;

namespace SWP.Controllers
{
    public class SizeController : Controller
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
                    var totalSizes = context.Sizes.Count();

                    var sizeList = context.Sizes
                        .OrderBy(s => s.SizeId)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var totalPages = (int)Math.Ceiling((double)totalSizes / pageSize);

                    ViewBag.CurrentPage = page;
                    ViewBag.TotalPages = totalPages;

                    return View(sizeList);
                }
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
                    ModelState.AddModelError("SizeName", "Tên Kích thước chỉ được chứa chữ cái hoặc số.");
                    // Xử lý lỗi nếu cần thiết
                    return View(newSize);
                }

                using (var context = new SWPContext())
                {
                    if (context.Sizes.Any(s => s.SizeName == newSize.SizeName))
                    {
                        ModelState.AddModelError("SizeName", "Tên kích thước đã tồn tại. Vui lòng chọn tên khác.");
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
            return !string.IsNullOrWhiteSpace(name) && Regex.IsMatch(name, "^[a-zA-Z0-9à-ỹẠ-Ỹ\\s]+$");
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
                    TempData["ErrorMessage"] = "Không thể xóa vì kích thước này đang có sản phẩm sử dụng.";
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
                        // Kiểm tra xem có sản phẩm sử dụng kích thước này không
                        if (HasProductDetailInSize(existingSize.SizeId))
                        {
                            // Nếu có sản phẩm sử dụng kích thước này, chỉ cho phép chỉnh sửa tên kích thước
                            existingSize.SizeName = editedSize.SizeName;
                        }
                        else
                        {
                            // Kiểm tra xem người dùng có chỉnh sửa cả trạng thái không
                            if (existingSize.Status != editedSize.Status)
                            {
                                // Nếu người dùng cố gắng chỉnh sửa cả trạng thái
                                TempData["ErrorMessage"] = "Không thể thay đổi trạng thái vì đang có sản phẩm sử dụng kích thước này.";
                                return RedirectToAction("Index");
                            }

                            // Kiểm tra xem tên mới của kích thước có trùng với bất kỳ tên nào khác trong cơ sở dữ liệu không
                            var sizeNameExists = context.Sizes.Any(c => c.SizeId != existingSize.SizeId && c.SizeName == editedSize.SizeName);
                            if (sizeNameExists)
                            {
                                ModelState.AddModelError("SizeName", "Tên kích thước đã tồn tại. Vui lòng chọn tên khác.");
                                return View(existingSize);
                            }

                            // Cho phép chỉnh sửa cả tên và trạng thái
                            existingSize.SizeName = editedSize.SizeName;
                            existingSize.Status = editedSize.Status;
                        }

                        context.SaveChanges();
                    }
                    else
                    {
                        // Xử lý khi không tìm thấy Size
                        return NotFound();
                    }
                }

                // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
                TempData["SuccessMessage"] = "Size đã được cập nhật thành công.";
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại trang Edit với thông tin nhập trước
            return View(editedSize);
        }
        private bool HasProductDetailInSize(int sizeId)
        {
            using (var context = new SWPContext())
            {
                // Kiểm tra xem có sản phẩm nào sử dụng Category có ID là categoryId không
                var HasProductDetailInSize = context.ProductDetails.Where(p => p.SizeId == sizeId).ToList();
                return HasProductDetailInSize.Any();
            }
        }
    }
}
