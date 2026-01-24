using TechStore.Web.ViewModels.OrderProduct;

namespace TechStore.Web.ViewModels.Order
{
    public class OrderDetailsViewModel
    {
        public long Id { get; set; }
        public string RecipientName { get; set; } = null!;
        public string Date { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public IEnumerable<OrderProductDetailsViewModel> Products { get; set; } = new List<OrderProductDetailsViewModel>();
        public decimal TotalSum => Products.Sum(p => p.TotalSum);
    }
}
