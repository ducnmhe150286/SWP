using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class CartItem
    {
        public int ProductId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }

        public virtual User Customer { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
