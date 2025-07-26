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

        public async Task<bool> ExistsByNameAsync(string name, string? productIdToSkip)
        {
            IQueryable<Product> query = this.GetAllAttached().AsNoTracking();

            if (!Guid.TryParse(productIdToSkip, out Guid idToExclude))
            {
                return await query
                    .AnyAsync(p => p.Name.ToLower() == name.ToLower());
            }

            return await query
                .Where(p => p.Id != idToExclude)
                .AnyAsync(p => p.Name.ToLower() == name.ToLower());
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
                .Where(p => EF.Functions.Like(p.Name, $"%{keyword}%"))
                .ToListAsync();

            return products;
        }
    }
}
