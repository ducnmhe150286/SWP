using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
