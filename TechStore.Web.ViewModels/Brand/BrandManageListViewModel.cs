namespace TechStore.Web.ViewModels.Brand
{
    public class BrandManageListViewModel
    {
        public IEnumerable<BrandManageViewModel> Brands { get; set; } = new List<BrandManageViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
