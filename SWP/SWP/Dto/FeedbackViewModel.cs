namespace SWP.Dto
{
    public class FeedbackViewModel
    {
        public int FeedBackId { get; set; }
        public string Email { get; set; }
        public string ProductName { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
