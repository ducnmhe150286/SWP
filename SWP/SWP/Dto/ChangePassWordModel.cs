using System.ComponentModel.DataAnnotations;

namespace SWP.Dto
{
    public class ChangePassWordModel
    {
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[@#$%^&+=!]).{8,}$", ErrorMessage = "Mật khẩu tối thiểu 8 ký tự (@,... và 1 ký tự in hoa))")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[@#$%^&+=!]).{8,}$", ErrorMessage = "Mật khẩu tối thiểu 8 ký tự (@,... và 1 ký tự in hoa))")]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu không khớp!")]
        public string ConfirmPassword { get; set; }
    }
}
