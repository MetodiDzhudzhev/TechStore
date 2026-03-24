using TechStore.Web.ViewModels.Cart;

namespace TechStore.Services.Core.Interfaces
{
    public interface ICartService
    {
        Task<CartViewModel?> GetCartAsync(Guid id);
        Task<bool> AddProductAsync(Guid cartId, Guid productId, int quantity);
        Task<bool> IncreaseProductQuantityAsync(Guid cartId, Guid productId);
        Task<bool> RemoveProductAsync(Guid cartId, Guid productId);
        Task<bool> DecreaseProductAsync(Guid cartId, Guid productId);
        Task<bool> ClearCartAsync(Guid cartId);
        Task<int> GetCartItemsCountAsync(Guid userId);
    }
}
