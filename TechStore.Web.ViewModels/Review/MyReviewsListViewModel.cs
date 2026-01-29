namespace TechStore.Web.ViewModels.Review
{
    public class MyReviewsListViewModel
    {
        public IEnumerable<MyReviewsItemViewModel> Reviews { get; set; } = new List<MyReviewsItemViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
