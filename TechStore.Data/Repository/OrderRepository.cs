using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using TechStore.Data.Models.Enums;
using TechStore.Data.Repository.Interfaces;

namespace TechStore.Data.Repository
{
    public class OrderRepository : BaseRepository<Order, long>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        {

        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(Status status)
        {
            ICollection<Order> orders = await this
                .GetAllAttached()
                .Where(o => o.Status == status)
                .Include(o => o.OrdersProducts)
                .ThenInclude(op => op.Product)
                .ToArrayAsync();

            return orders;
        }

        public async Task<IEnumerable<Order>> GetByUserAsync(Guid userId)
        {
            ICollection<Order> orders = await this
                .GetAllAttached()
                .Where(o => o.UserId == userId)
                .Include(o => o.OrdersProducts)
                .ThenInclude(op => op.Product)
                .ToListAsync();

            return orders;
        }

        public async Task<Order?> GetOrderDetailsAsync(Guid userId, long orderId)
        {
            return await this
                .GetAllAttached()
                .Include(o => o.OrdersProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        }

        public async Task<IReadOnlyList<Order>> GetPagedByUserAsync(Guid userId, int page, int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 5;
            }

            int skip = (page - 1) * pageSize;

            return await this
                .GetAllAttached()
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Skip(skip)
                .Take(pageSize)
                .Include(o => o.OrdersProducts)
                .ThenInclude(op => op.Product)
                .ToListAsync();
        }

        public async Task<int> GetCountByUserAsync(Guid userId)
        {
            return await this
                .GetAllAttached()
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .CountAsync();
        }
    }
}
