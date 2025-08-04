namespace TechStore.Web.ViewModels.Product
{
    public class ProductManageListViewModel
    {
        public IEnumerable<ProductManageViewModel> Products { get; set; } = new List<ProductManageViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
