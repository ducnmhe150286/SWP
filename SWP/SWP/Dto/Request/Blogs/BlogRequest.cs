using System.ComponentModel.DataAnnotations;

namespace SWP.Dto.Request.Blogs
{
    public class BlogRequest
    {
        public int BlogId { get; set; }
        public int? UserId { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

    }
    public class CreateBlogRequest
    {

        public int BlogId { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Ảnh không được để trống.")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        public string Description { get; set; }
        public string? ShortDescription { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? Status { get; set; }
    }

    }
