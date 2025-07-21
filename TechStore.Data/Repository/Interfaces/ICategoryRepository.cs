using TechStore.Data.Models;

namespace TechStore.Data.Repository.Interfaces
{
    public interface ICategoryRepository 
        : IRepository<Category, int>, IAsyncRepository<Category, int>
    {
        Task<bool> ExistsAsync(int id);
    }
}
