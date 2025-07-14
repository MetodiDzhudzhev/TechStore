using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;

namespace TechStore.Data.Repository
{
    public class CategoryRepository : BaseRepository<Category, int>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        {

        }
    }
}
