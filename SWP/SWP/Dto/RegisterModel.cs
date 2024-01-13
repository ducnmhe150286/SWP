﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace SWP.Dto
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Email không đúng!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z]).{8,}$", ErrorMessage = "Mật khẩu tối thiểu 8 ký tự (@,... và 1 ký tự in hoa))")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp!")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "SĐT không được để trống!")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "SĐT không đúng!")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Tên không được để trống!")]
        [MaxLength(30, ErrorMessage = "Không được vượt quá 30 ký tự!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Họ không được để trống!")]
        [MaxLength(30, ErrorMessage = "Không được vượt quá 30 ký tự!")]
        public string LastName { get; set; }

        public ModelStateDictionary ModelState { get; set; }
    }
}
