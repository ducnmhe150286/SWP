using System.ComponentModel.DataAnnotations;

namespace SWP.Dto
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Email không đúng!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[@#$%^&+=!]).{8,}$", ErrorMessage = "Mật khẩu tối thiểu 8 ký tự (@,... và 1 ký tự in hoa)")]
        public string Password { get; set; }
    }
}
