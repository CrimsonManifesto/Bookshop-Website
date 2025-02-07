namespace Bookshop_Website.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? UserPhone { get; set; }
        public string? UserEmail { get; set; }
        public string FeedbackType { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
