using TechStore.Data.Models;

namespace TechStore.Data.Repository.Interfaces
{
    public interface IBrandRepository
        : IRepository<Brand, int>, IAsyncRepository<Brand, int>
    {
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int brandIdToSkip);
        Task<Brand?> GetDeletedBrandByNameAsync(string name);
    }
}
