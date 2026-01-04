namespace TechStore.Web.ViewModels.CartProduct
{
    public class CartProductDetailsViewModel
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
