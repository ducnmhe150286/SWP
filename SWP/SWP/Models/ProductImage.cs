using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class ProductImage
    {
        public int ProductImageId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }
        public string? Path { get; set; }
        public int? ProductDetailId { get; set; }

        public virtual ProductDetail? ProductDetail { get; set; }
    }
}
