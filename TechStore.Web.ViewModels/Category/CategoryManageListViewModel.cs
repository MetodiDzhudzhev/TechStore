namespace TechStore.Web.ViewModels.Category
{
    public class CategoryManageListViewModel
    {
        public IEnumerable<CategoryManageViewModel> Categories { get; set; } = new List<CategoryManageViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
