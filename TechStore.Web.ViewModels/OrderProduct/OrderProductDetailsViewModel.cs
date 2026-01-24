namespace TechStore.Web.ViewModels.OrderProduct
{
    public class OrderProductDetailsViewModel
    {
        public string ProductName { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalSum => Price * Quantity;
    }
}
