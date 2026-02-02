namespace TechStore.Web.ViewModels.Review
{
    public class ReviewManageListViewModel
    {
        public IEnumerable<ReviewManageItemViewModel> Reviews { get; set; } = new List<ReviewManageItemViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
