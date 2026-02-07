using TechStore.Web.ViewModels.Cart;

namespace TechStore.Services.Core.Interfaces
{
    public interface ICartService
    {
        Task<CartViewModel?> GetCartAsync(string? id);
        Task<bool> AddProductAsync(string cartId, string? productId, int quantity);
        Task<bool> IncreaseProductQuantityAsync(string cartId, string? productId);
        Task<bool> RemoveProductAsync(string cartId, string? productId);
        Task<bool> DecreaseProductAsync(string cartId, string? productId);
        Task<bool> ClearCartAsync(string? cartId);
        Task<int> GetCartItemsCountAsync(Guid userId);
    }
}
