using TechStore.Web.ViewModels.Cart;

namespace TechStore.Services.Core.Interfaces
{
    public interface ICartService
    {
        Task<CartViewModel?> GetCartAsync(string? id);
        Task<bool> AddProductAsync(string cartId, string? productId, int quantity);
    }
}
