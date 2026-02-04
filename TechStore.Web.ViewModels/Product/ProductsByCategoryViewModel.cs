namespace TechStore.Web.ViewModels.Product
{
    public class ProductsByCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public IEnumerable<ProductInCategoryViewModel> Products { get; set; } = new List<ProductInCategoryViewModel>();
        public ProductSort Sort { get; set; } = ProductSort.NameAsc;
    }
}