using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class Color
    {
        public Color()
        {
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int ColorId { get; set; }
        public string? ColorName { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
