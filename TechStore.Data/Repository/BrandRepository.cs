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
    }
}
