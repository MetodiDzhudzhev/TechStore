namespace TechStore.Web.ViewModels.Category
{
    public class CategoryManageListViewModel
    {
        public IEnumerable<CategoryManageViewModel> Categories { get; set; } = new List<CategoryManageViewModel>();
    }
}
