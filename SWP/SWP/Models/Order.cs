using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class Order
    {
        public Order()
        {
            Orderdetails = new HashSet<Orderdetail>();
        }

        public string OrderId { get; set; } = null!;
        public int? UserId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Orderdetail> Orderdetails { get; set; }
    }
}
