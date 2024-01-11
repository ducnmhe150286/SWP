using System;
using System.Collections.Generic;

namespace SWP.Models
{
    public partial class FeedBack
    {
        public int FeedBackId { get; set; }
        public int? UserId { get; set; }
        public int? DetailId { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
