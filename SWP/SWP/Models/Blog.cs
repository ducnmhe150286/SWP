using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class Blog
    {
        public int BlogId { get; set; }
        public int? UserId { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
