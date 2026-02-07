using TechStore.Data.Models;

namespace TechStore.Data.Repository.Interfaces
{
    public interface ICartRepository : IRepository<Cart, Guid>, IAsyncRepository<Cart, Guid>
    {
        Task<Cart?> GetCartWithProductsAsync(Guid cartId);
        Task<int> GetCartItemsCountAsync(Guid cartId);
    }
}
