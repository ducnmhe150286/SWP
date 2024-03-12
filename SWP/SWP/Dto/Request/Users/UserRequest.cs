using System.ComponentModel.DataAnnotations;

namespace SWP.Dto.Request.Users
{
    public class UserRequest
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public bool? Gender { get; set; }
        public int? Status { get; set; }
        public string? Image { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class CreateUserRequest
    {
        public int RoleId { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^(?!.*\b\d+\b)[A-Za-z0-9]+$", ErrorMessage = "First Name may contain letters and numbers, but cannot contain numbers only")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^(?!.*\b\d+\b)[A-Za-z0-9]+$", ErrorMessage = "Last Name may contain letters and numbers, but cannot contain numbers only")]
        public string? LastName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Invalid email format. Email must be from Gmail.com")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?\/~`-])[A-Za-z\d!@#$%^&*()_+{}\[\]:;<>,.?\/~`-]{8,}$",
            ErrorMessage = "Password must be at least 8 characters, include at least one uppercase letter, and contain at least one special character.")]
        public string? Password { get; set; }


        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Invalid Phone Number format")]
        public string? PhoneNumber { get; set; }
        public int? Status { get; set; }
    }
}
