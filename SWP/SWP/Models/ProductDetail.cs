using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class ProductDetail
    {
        public ProductDetail()
        {
            CartItems = new HashSet<CartItem>();
        }

        public int DetailId { get; set; }
        public int? ProductId { get; set; }
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Quantity { get; set; }

        public virtual Color? Color { get; set; }
        public virtual Product? Product { get; set; }
        public virtual Size? Size { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
