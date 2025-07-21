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
        }
    }
