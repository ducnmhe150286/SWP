using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Products = new HashSet<Product>();
        }

        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
