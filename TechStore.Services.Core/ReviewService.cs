using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Review;

namespace TechStore.Services.Core
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository reviewRepository;
        private readonly IUserRepository userRepository;

        public ReviewService(IReviewRepository reviewRepository,
            IUserRepository userRepository)
        {
            this.reviewRepository = reviewRepository;
            this.userRepository = userRepository;
        }

        public async Task<bool> Add(string userId, ReviewFormInputModel inputModel)
        {
            User? user = await userRepository.GetByIdAsync(Guid.Parse(userId));

            if (user == null)
            {
                return false;
            }

            bool exist = await reviewRepository.ReviewExistsAsync(user.Id, inputModel.ProductId); 

            if (exist)
            {
                return false;
            }

            Review review = new Review
            {
                Rating = inputModel.Rating!.Value,
                Comment = inputModel.Comment,
                CreatedAt = DateTime.UtcNow,
                ProductId = inputModel.ProductId,
                UserId = user.Id,
            };

            await this.reviewRepository.AddAsync(review);
            await this.reviewRepository.SaveChangesAsync();

            return true;
        }

        public async Task<ReviewsPanelViewModel> GetPanelAsync(Guid productId, Guid? userId, int page, int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 5;
            }

            int totalCount = await reviewRepository.GetCountByProductAsync(productId);
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (page > totalPages)
            {
                page = totalPages;
            }

            IReadOnlyList<Review> reviews = await reviewRepository.GetPagedByProductAsync(productId, page, pageSize);

            bool canAdd = false;

            if (userId.HasValue)
            {
                bool hasActive = await reviewRepository.ReviewExistsAsync(userId.Value, productId);
                canAdd = !hasActive;
            }

            double averageRating = 0;

            if (totalCount > 0)
            {
                averageRating = await reviewRepository.GetAverageRatingByProductAsync(productId);
            }

            return new ReviewsPanelViewModel
            {
                ProductId = productId,
                CanAddReview = canAdd,
                TotalCount = totalCount,
                AverageRating = averageRating,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                Items = reviews.Select(r => new ReviewListItemViewModel
                {
                    UserName = r.User.FullName ?? r.User.Email ?? "Anonymous",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList()
            };
        }

        public async Task<ReviewsStatsViewModel> GetStatsAsync(Guid productId)
        {
            int totalCount = await reviewRepository.GetCountByProductAsync(productId);
            double average = await reviewRepository.GetAverageRatingByProductAsync(productId);

            return new ReviewsStatsViewModel
            {
                TotalCount = totalCount,
                AverageRating = average
            };
        }

    }
}
