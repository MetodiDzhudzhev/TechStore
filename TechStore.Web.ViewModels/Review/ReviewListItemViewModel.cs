namespace TechStore.Web.ViewModels.Review
{
    public class ReviewListItemViewModel
    {
        public string UserName { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }    
        public DateTime CreatedAt { get; set; }
    }
}
