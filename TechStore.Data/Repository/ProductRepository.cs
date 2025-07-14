using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;

namespace TechStore.Data.Repository
{
    public class ProductRepository : BaseRepository<Product, Guid>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        {

        }

        public async Task<IEnumerable<Product>> GetByBrandAsync(int brandId)
        {
            ICollection<Product> products = await this
                .GetAllAttached()
                .AsNoTracking()
                .Where(p => p.BrandId == brandId)
                .ToListAsync();

            return products;
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            ICollection<Product> products = await this
                .GetAllAttached()
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            return products;
        }

        public async Task<IEnumerable<Product>> SearchByKeywordAsync(string keyword)
        {
            ICollection<Product> products = await this
                .GetAllAttached()
                .AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(keyword.ToLower()))
                .ToListAsync();

            return products;
        }
    }
}
