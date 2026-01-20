using TechStore.Data.Models;
using TechStore.Web.ViewModels.Order;
using TechStore.Web.ViewModels.Payment;

namespace TechStore.Services.Core.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDeliveryDetailsViewModel?> GetCheckoutDeliveryDetailsAsync(string userId);
        Task<long?> CreateOrderAsync(string userId, CreateOrderViewModel model);
        Task<PaymentSummaryViewModel?> GetPaymentSummaryAsync(string userId, long orderId);
        Task<bool> AttachStripeSessionAsync(long orderId, string sessionId);
        Task<PaymentSuccessViewModel?> GetPaymentSuccessAsync(string userId, long orderId);
        Task<List<OrderProduct>?> GetOrderProductsAsync(string userId, long orderId);
        Task MarkOrderAsPaidByOrderIdAsync(long orderId);
        Task<bool> MarkOrderAsCashOnDeliveryAsync(string userId, long orderId);
    }
}
