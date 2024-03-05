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

        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? ShipAddress { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public string? PhoneNumber { get; set; }
        public byte? Status { get; set; }
        public byte? PaymentMethod { get; set; }

        public virtual ICollection<Orderdetail> Orderdetails { get; set; }
    }
}
