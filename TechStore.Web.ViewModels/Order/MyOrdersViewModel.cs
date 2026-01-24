namespace TechStore.Web.ViewModels.Order
{
    public class MyOrdersViewModel
    {
        public IEnumerable<OrderDetailsViewModel> Orders { get; set; } = new List<OrderDetailsViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
