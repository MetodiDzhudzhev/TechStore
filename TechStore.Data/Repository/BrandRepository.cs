using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;

namespace TechStore.Data.Repository
{
    public class BrandRepository : BaseRepository<Brand, int>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        {

        }

        public async Task<bool> ExistsAsync(int id)
        {
            bool result = false;

            Brand? brand = await this.GetByIdAsync(id);

            if (brand != null)
            {
                result = true;
            }
            return result;
        }

        public async Task<bool> ExistsByNameAsync(string name, int brandIdToSkip)
        {
            IQueryable<Brand> query = this
                .GetAllAttached()
                .IgnoreQueryFilters()
                .AsNoTracking();

            return await query
                .Where(b => b.Id != brandIdToSkip)
                .AnyAsync(b => b.Name.ToLower() == name.ToLower());
        }

        public async Task<Brand?> GetDeletedBrandByNameAsync(string name)
        {
            return await this
                .GetAllAttached()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Name.ToLower() == name.ToLower() && b.IsDeleted == true);
        }
    }
}
