﻿using TechStore.Data.Models;

namespace TechStore.Data.Repository.Interfaces
{
    public interface IReviewRepository
        : IRepository<Review, long>, IAsyncRepository<Review, long>
    {
        Task<IEnumerable<Review>> GetByUserAsync(Guid userId);
        Task<IEnumerable<Review>> GetByProductAsync(Guid productId);
        Task<bool> ReviewExistsAsync(Guid userId, Guid productId);

    }
}
