using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class User
    {
        public User()
        {
            Blogs = new HashSet<Blog>();
            FeedBacks = new HashSet<FeedBack>();
            Orders = new HashSet<Order>();
        }

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
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Image { get; set; }
       

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<FeedBack> FeedBacks { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
