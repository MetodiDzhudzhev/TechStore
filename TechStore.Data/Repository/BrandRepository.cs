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
    }
}
