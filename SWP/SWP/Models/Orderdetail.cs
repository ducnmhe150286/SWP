using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class Orderdetail
    {
        public int OrderId { get; set; }
        public int DetailId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}
