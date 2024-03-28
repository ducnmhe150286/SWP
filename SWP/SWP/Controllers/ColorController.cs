using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Models;
using System.Text.RegularExpressions;

namespace SWP.Controllers
{
    public class ColorController : Controller
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
                    var totalColors = context.Colors.Count();

                    var colorList = context.Colors
                        .OrderBy(c => c.ColorId)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var totalPages = (int)Math.Ceiling((double)totalColors / pageSize);

                    ViewBag.CurrentPage = page;
                    ViewBag.TotalPages = totalPages;

                    return View(colorList);
                }
            }
        }
        [HttpGet]
        public ActionResult AddColor()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddColor(Color newColor)
        {
            if (ModelState.IsValid)
            {
                if (!IsValidName(newColor.ColorName))
                {
                    ModelState.AddModelError("ColorName", "Tên Color chỉ được chứa chữ cái.");
                    // Xử lý lỗi nếu cần thiết
                    return View(newColor);
                }

                using (var context = new SWPContext())
                {
                    if (context.Colors.Any(c => c.ColorName == newColor.ColorName))
                    {
                        ModelState.AddModelError("ColorName", "Tên Color đã tồn tại. Vui lòng chọn tên khác.");
                        // Xử lý lỗi nếu cần thiết
                        return View(newColor);
                    }
                    // Lấy tên người dùng hiện tại, bạn cần cập nhật logic lấy tên người dùng theo cách của bạn
                    string currentUserName = "User1"; // Thay thế bằng logic lấy tên người dùng

                    // Chuyển đổi giá trị từ kiểu string sang kiểu int?
                    newColor.Status = 1;

                    // Tạo mới Category
                    Color colorToAdd = new Color
                    {
                        ColorName = newColor.ColorName,
                        CreatedBy = currentUserName,
                        UpdatedBy = currentUserName,
                        Status = newColor.Status,
                        CreatedDate = DateTime.Now
                    };

                    // Thêm mới Category vào database
                    context.Colors.Add(colorToAdd);
                    context.SaveChanges();
                }

                // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại trang AddCategory với thông tin nhập trước đó
            return View(newColor);
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
                var colorToDelete = context.Colors.Include(c => c.ProductDetails).FirstOrDefault(c => c.ColorId == id);

                if (colorToDelete == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem có sản phẩm đang sử dụng Category hay không
                if (colorToDelete.ProductDetails.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa vì thương hiệu đang có sản phẩm sử dụng.";
                    return RedirectToAction(nameof(Index));
                }

                context.Colors.Remove(colorToDelete);
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
                var colorToEdit = context.Colors.Find(id);
                if (colorToEdit == null)
                {
                    // Xử lý khi không tìm thấy Category
                    return NotFound();
                }

                return View(colorToEdit);
            }
        }

        [HttpPost]
        public IActionResult Edit(Color editedColor)
        {
            if (ModelState.IsValid)
            {
                using (var context = new SWPContext())
                {
                    var existingColor = context.Colors.Find(editedColor.ColorId);

                    if (existingColor != null)
                    {

                        // Kiểm tra xem có sản phẩm sử dụng màu sắc không
                        if (HasProductDetailInColor(existingColor.ColorId))
                        {
                            // Nếu có sản phẩm sử dụng màu sắc này, chỉ cho phép chỉnh sửa tên màu sắc
                            existingColor.ColorName = editedColor.ColorName;
                        }
                        else
                        {
                            // Kiểm tra xem người dùng có chỉnh sửa cả trạng thái không
                            if (existingColor.Status != editedColor.Status)
                            {
                                // Nếu người dùng cố gắng chỉnh sửa cả trạng thái
                                TempData["ErrorMessage"] = "Không thể thay đổi trạng thái vì đang có sản phẩm sử dụng màu sắc này.";
                                return RedirectToAction("Index");
                            }

                            // Kiểm tra xem tên mới của màu sắc có trùng với bất kỳ tên nào khác trong cơ sở dữ liệu không
                            var colorNameExists = context.Colors.Any(c => c.ColorId != existingColor.ColorId && c.ColorName == editedColor.ColorName);
                            if (colorNameExists)
                            {
                                ModelState.AddModelError("ColorName", "Tên màu sắc đã tồn tại. Vui lòng chọn tên khác.");
                                return View(existingColor);
                            }

                            // Cho phép chỉnh sửa cả tên và trạng thái
                            existingColor.ColorName = editedColor.ColorName;
                            existingColor.Status = editedColor.Status;
                        }

                        context.SaveChanges();
                    }
                    else
                    {
                        // Xử lý khi không tìm thấy Color
                        return NotFound();
                    }
                }

                // Chuyển hướng đến trang danh sách (Index) để hiển thị danh sách cập nhật
                TempData["SuccessMessage"] = "Màu sắc đã được cập nhật thành công.";
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại trang Edit với thông tin nhập trước
            return View(editedColor);
        }
        private bool HasProductDetailInColor(int colorId)
        {
            using (var context = new SWPContext())
            {
                // Kiểm tra xem có sản phẩm nào sử dụng Category có ID là categoryId không
                var productDetailInColor = context.ProductDetails.Where(p => p.ColorId == colorId).ToList();
                return productDetailInColor.Any();
            }
        }
    }
}
