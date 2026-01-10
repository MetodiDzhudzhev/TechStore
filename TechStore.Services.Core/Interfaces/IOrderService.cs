using TechStore.Web.ViewModels.Order;

namespace TechStore.Services.Core.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDeliveryDetailsViewModel?> GetOrderDeliveryDetailsAsync(string userId);
    }
}
