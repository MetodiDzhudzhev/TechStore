namespace TechStore.Web.ViewModels.Brand
{
    public class BrandManageListViewModel
    {
        public IEnumerable<BrandManageViewModel> Brands { get; set; } = new List<BrandManageViewModel>();
    }
}
