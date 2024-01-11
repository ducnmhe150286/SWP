using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class Size
    {
        public Size()
        {
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int SizeId { get; set; }
        public string? SizeName { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
