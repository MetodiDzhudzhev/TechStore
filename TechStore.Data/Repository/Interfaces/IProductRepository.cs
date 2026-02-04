using TechStore.Data.Models;

namespace TechStore.Data.Repository.Interfaces
{
    public interface IProductRepository 
        : IRepository<Product, Guid>, IAsyncRepository<Product, Guid>
    {
        public IQueryable<Product> GetByCategoryQuery(int categoryId);
        Task<IEnumerable<Product>> GetByBrandAsync(int brandId);
        Task<IEnumerable<Product>> SearchByKeywordAsync(string keyword);
        Task<bool> ExistsByNameAsync(string name, string? productIdToSkip);
        Task<Product?> GetDeletedProductByNameAsync(string name);

    }
}
