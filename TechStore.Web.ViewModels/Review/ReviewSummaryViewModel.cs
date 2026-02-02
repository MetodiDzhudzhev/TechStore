namespace TechStore.Web.ViewModels.Review
{
    public class ReviewSummaryViewModel
    {
        public long ReviewId { get; set; }
        public int Rating { get; set; }
        public string CreatedAt { get; set; } = null!;
        public string? Comment { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductImage { get; set; }
    }
}
