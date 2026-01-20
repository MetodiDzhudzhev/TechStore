namespace TechStore.Web.ViewModels.Payment
{
    public class PaymentSuccessViewModel
    {
        public long OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<TechStore.Data.Models.OrderProduct> Products { get; set; } = new();
    }
}
