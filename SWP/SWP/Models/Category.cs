using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
            Sizes = new HashSet<Size>();
        }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Size> Sizes { get; set; }
    }
}
