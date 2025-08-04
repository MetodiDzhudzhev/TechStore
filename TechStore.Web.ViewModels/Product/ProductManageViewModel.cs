namespace TechStore.Web.ViewModels.Product
{
    public class ProductManageViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
    }
}
