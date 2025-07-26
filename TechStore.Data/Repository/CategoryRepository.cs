using Microsoft.EntityFrameworkCore;
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

            public async Task<bool> ExistsAsync(int id)
            {
                bool result = false;

                Category? category = await this.GetByIdAsync(id);

                if (category != null)
                {
                    result = true;
                }
                return result;
            }

        public async Task<bool> ExistsByNameAsync(string name, int categoryIdToSkip)
        {
            IQueryable<Category> query = this.GetAllAttached().AsNoTracking();

            return await query
                .Where(c => c.Id != categoryIdToSkip)
                .AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }

    }
}
