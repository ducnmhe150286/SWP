namespace SWP.Dto.Request.Users
{
    public class UserRequest
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? Gender { get; set; }
        public int? Status { get; set; }
        public string? Image { get; set; }
    }
}
