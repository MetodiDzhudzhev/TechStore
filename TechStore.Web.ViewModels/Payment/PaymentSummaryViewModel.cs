namespace TechStore.Web.ViewModels.Payment
{
    public class PaymentSummaryViewModel
    {
        public long OrderId { get; set; }
        public string RecipientName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
    }
}
