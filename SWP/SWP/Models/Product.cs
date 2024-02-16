using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int BrandId { get; set; }
        public decimal? Price { get; set; }
        public int CategoryId { get; set; }
        public bool? IsSale { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }

        public virtual Brand Brand { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
