namespace TechStore.Web.ViewModels.Review
{
    public class ReviewsPanelViewModel
    {
        public Guid ProductId { get; set; }
        public bool CanAddReview { get; set; }
        public int TotalCount { get; set; }
        public double AverageRating { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public IReadOnlyList<ReviewListItemViewModel> Items { get; set; } = new List<ReviewListItemViewModel>();
    }
}
