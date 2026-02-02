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

        public async Task<IReadOnlyList<Review>> GetPagedByProductAsync(Guid productId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;

            return await this.GetAllAttached()
                .AsNoTracking()
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountByProductAsync(Guid productId)
        {
            return await this.GetAllAttached()
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .CountAsync();
        }

        public async Task<double> GetAverageRatingByProductAsync(Guid productId)
        {
            return await this.GetAllAttached()
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .Select(r => (double?)r.Rating)
                .AverageAsync() ?? 0;
        }

        public async Task<IReadOnlyList<Review>> GetPagedByUserAsync(Guid userId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;

            return await this
                .GetAllAttached()
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Include(r => r.Product)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Review>> GetPagedAsync(int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;

            return await this
                .GetAllAttached()
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountByUserAsync(Guid userId)
        {
            return await this
                .GetAllAttached()
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .CountAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await this
               .GetAllAttached()
               .AsNoTracking()
               .CountAsync();
        }
    }
}
