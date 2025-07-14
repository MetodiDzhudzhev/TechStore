using TechStore.Data.Models;

namespace TechStore.Data.Repository.Interfaces
{
    public interface IProductRepository 
        : IRepository<Product, Guid>, IAsyncRepository<Product, Guid>
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetByBrandAsync(int brandId);
        Task<IEnumerable<Product>> SearchByKeywordAsync(string keyword);

    }
}
