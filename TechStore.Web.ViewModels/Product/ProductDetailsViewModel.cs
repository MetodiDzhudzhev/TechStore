namespace TechStore.Web.ViewModels.Product
{
    public class ProductDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public string Brand { get; set; } = null!;

        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }

        public int CategoryId { get; set; }
    }
}
