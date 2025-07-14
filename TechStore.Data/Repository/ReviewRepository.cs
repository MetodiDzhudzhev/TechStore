using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;

namespace TechStore.Data.Repository
{
    public class ReviewRepository : BaseRepository<Review, long>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext dbContext) 
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<Review>> GetByProductAsync(Guid productId)
        {
            ICollection<Review> reviews = await this
                .GetAllAttached()
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            return reviews;
        }

        public async Task<IEnumerable<Review>> GetByUserAsync(Guid userId)
        {
            ICollection<Review> reviews = await this
                .GetAllAttached()
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return reviews;
        }

        public Task<bool> ReviewExistsAsync(Guid userId, Guid productId)
        {
            var result = this
                .GetAllAttached()
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId);

            return result;
        }
    }
}
