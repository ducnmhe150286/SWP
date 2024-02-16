using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class ProductDetail
    {
        public ProductDetail()
        {
            ProductImages = new HashSet<ProductImage>();
        }

        public int DetailId { get; set; }
        public int? ProductId { get; set; }
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Feature { get; set; }
        public string? Attributes { get; set; }
        public int? Quantity { get; set; }

        public virtual Color? Color { get; set; }
        public virtual Product? Product { get; set; }
        public virtual Size? Size { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}
