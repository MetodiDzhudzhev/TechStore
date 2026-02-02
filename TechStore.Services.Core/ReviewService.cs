using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Review;
using static TechStore.GCommon.ValidationConstants.Review;

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
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

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

        public async Task<MyReviewsListViewModel> GetMyReviewsPagedAsync(Guid userId, int page, int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 5;
            }

            int totalCount = await reviewRepository.GetCountByUserAsync(userId);
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

            if (page > totalPages)
            {
                page = totalPages;
            }

            IReadOnlyList<Review> reviews = await reviewRepository.GetPagedByUserAsync(userId, page, pageSize);

            List<MyReviewsItemViewModel> mappedReviews = reviews.Select(r => new MyReviewsItemViewModel
            {
                ReviewId = r.Id,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt.ToString(CreatedAtFormat),
                Comment = r.Comment,
                ProductId = r.ProductId,
                ProductName = r.Product.Name,
                ProductImage = r.Product.ImageUrl
            }).ToList();

            return new MyReviewsListViewModel
            {
                Reviews = mappedReviews,
                CurrentPage = page,
                TotalPages = totalPages,
            };
        }

        public async Task<ReviewManageListViewModel> GetManageReviewsPageAsync(int page, int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 5;
            }

            int totalCount = await reviewRepository.GetCountAsync();
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

            if (page > totalPages)
            {
                page = totalPages;
            }

            IReadOnlyList<Review> reviews = await reviewRepository.GetPagedAsync(page, pageSize);

            List<ReviewManageItemViewModel> mappedReviews = reviews.Select(r => new ReviewManageItemViewModel
            {
                Author = r.User.FullName ?? r.User.Email ?? "Anonymous",
                ReviewId = r.Id,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt.ToString(CreatedAtFormat),
                Comment = r.Comment,
                ProductId = r.ProductId,
                ProductName = r.Product.Name,
                ProductImage = r.Product.ImageUrl
            }).ToList();

            return new ReviewManageListViewModel
            {
                Reviews = mappedReviews,
                CurrentPage = page,
                TotalPages = totalPages,
            };
        }

        public async Task<bool> SoftDeleteReviewAsync(long reviewId)
        {
            Review? review = await reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                return false;
            }

            this.reviewRepository.Delete(review);
            await reviewRepository.SaveChangesAsync();

            return true;
        }

    }
}
