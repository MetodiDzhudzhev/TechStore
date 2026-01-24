using TechStore.Data.Models;
using TechStore.Data.Models.Enums;

namespace TechStore.Data.Repository.Interfaces
{
    public interface IOrderRepository
        : IRepository<Order, long>, IAsyncRepository<Order, long>
    {
        Task<IEnumerable<Order>> GetByUserAsync(Guid userId);
        Task<IEnumerable<Order>> GetByStatusAsync(Status status);
        Task<Order?> GetOrderDetailsAsync(Guid userId, long id);
        Task<IReadOnlyList<Order>> GetPagedByUserAsync(Guid userId, int page, int pageSize);
        Task<int> GetCountByUserAsync(Guid userId);
    }
}
