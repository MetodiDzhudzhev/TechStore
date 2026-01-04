using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;

namespace TechStore.Data.Repository
{
    public class CartRepository : BaseRepository<Cart, Guid>, ICartRepository
    {
        public CartRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {

        }

        public async Task<Cart?> GetCartWithProductsAsync(Guid cartId)
        {
            return await this
                .GetAllAttached()
                .Include(c => c.Products)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }
    }
}
