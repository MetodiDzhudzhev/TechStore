namespace TechStore.Web.ViewModels.Product
{
    public class AllProductsByCategoryIdViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
    }
}
