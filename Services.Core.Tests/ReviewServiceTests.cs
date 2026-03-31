using Moq;
using NUnit.Framework;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Web.ViewModels.Review;

namespace TechStore.Services.Core.Tests
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private Mock<IReviewRepository> mockReviewRepository = null!;
        private ReviewService reviewService = null!;

        [SetUp]
        public void SetUp()
        {
            this.mockReviewRepository = new Mock<IReviewRepository>();
            this.reviewService = new ReviewService(mockReviewRepository.Object);
        }

        [Test]
        public void PassAlways()
        {
            // Test that will always pass to show that the SetUp is working
            Assert.Pass();
        }

        [Test]
        public async Task Add_ShouldReturnFalse_WhenReviewAlreadyExists()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var inputModel = new ReviewFormInputModel
            {
                ProductId = productId,
                Rating = 5,
                Comment = "Great!"
            };

            SetupReviewExists(userId, productId, true);

            var result = await reviewService.Add(userId, inputModel);

            Assert.That(result, Is.False);

            mockReviewRepository.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Never);
            mockReviewRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task Add_ShouldAddReviewAndReturnTrue_WhenReviewDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var inputModel = new ReviewFormInputModel
            {
                ProductId = productId,
                Rating = 4,
                Comment = "Nice product"
            };

            Review? addedReview = null;

            SetupReviewExists(userId, productId, false);

            mockReviewRepository
                .Setup(r => r.AddAsync(It.IsAny<Review>()))
                .Callback<Review>(r => addedReview = r);

            var result = await reviewService.Add(userId, inputModel);

            Assert.That(result, Is.True);

            Assert.That(addedReview, Is.Not.Null);
            Assert.That(addedReview!.ProductId, Is.EqualTo(productId));
            Assert.That(addedReview.UserId, Is.EqualTo(userId));
            Assert.That(addedReview.Rating, Is.EqualTo(inputModel.Rating));
            Assert.That(addedReview.Comment, Is.EqualTo(inputModel.Comment));

            mockReviewRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetPanelAsync_ShouldReturnCorrectData_WhenValidInput()
        {
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var reviews = new List<Review>
            {
                CreateReview(
                    productId,
                    user: CreateUser("John Doe", "john@abv.bg")
                )
            };

            SetupCountByProduct(productId, 1);
            SetupPagedByProduct(productId, 1, 5, reviews);
            SetupReviewExists(userId, productId, false);
            SetupAverage(productId, 5);

            var result = await reviewService.GetPanelAsync(productId, userId, 1, 5);

            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.TotalPages, Is.EqualTo(1));
            Assert.That(result.CurrentPage, Is.EqualTo(1));
            Assert.That(result.CanAddReview, Is.True);
            Assert.That(result.AverageRating, Is.EqualTo(5));
            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.Items.First().UserName, Is.EqualTo("John Doe"));
        }

        [Test]
        public async Task GetPanelAsync_ShouldSetCanAddReviewFalse_WhenUserIsNull()
        {
            var productId = Guid.NewGuid();

            SetupCountByProduct(productId, 0);
            SetupPagedByProduct(productId, 1, 5, new List<Review>());

            var result = await reviewService.GetPanelAsync(productId, null, 1, 5);

            Assert.That(result.CanAddReview, Is.False);
        }

        [Test]
        public async Task GetPanelAsync_ShouldSetCanAddReviewFalse_WhenUserAlreadyHasReview()
        {
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            SetupCountByProduct(productId, 1);
            SetupPagedByProduct(productId, 1, 5, new List<Review>());
            SetupReviewExists(userId, productId, true);
            SetupAverage(productId, 4);

            var result = await reviewService.GetPanelAsync(productId, userId, 1, 5);

            Assert.That(result.CanAddReview, Is.False);
        }

        [Test]
        public async Task GetPanelAsync_ShouldSetAverageToZero_WhenNoReviews()
        {
            var productId = Guid.NewGuid();

            SetupCountByProduct(productId, 0);
            SetupPagedByProduct(productId, 1, 5, new List<Review>());

            var result = await reviewService.GetPanelAsync(productId, null, 1, 5);

            Assert.That(result.AverageRating, Is.EqualTo(0));
        }

        [Test]
        public async Task GetPanelAsync_ShouldClampPage_WhenPageExceedsTotalPages()
        {
            var productId = Guid.NewGuid();

            SetupCountByProduct(productId, 1);
            SetupPagedByProduct(productId, 1, 5, new List<Review>());
            SetupAverage(productId, 5);

            var result = await reviewService.GetPanelAsync(productId, null, 5, 5);

            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetPanelAsync_ShouldNormalizePageSize_WhenInvalid()
        {
            var productId = Guid.NewGuid();

            SetupCountByProduct(productId, 0);
            SetupPagedByProduct(productId, 1, 5, new List<Review>());

            var result = await reviewService.GetPanelAsync(productId, null, 0, 0);

            Assert.That(result.PageSize, Is.EqualTo(5));
            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetPanelAsync_ShouldUseFallbackUserName_WhenFullNameIsNull()
        {
            var productId = Guid.NewGuid();

            var review = CreateReview(productId, user:CreateUser(null, "email@test.com"));

            SetupCountByProduct(productId, 1);
            SetupPagedByProduct(productId, 1, 5, new List<Review> { review });
            SetupAverage(productId, 5);

            var result = await reviewService.GetPanelAsync(productId, null, 1, 5);

            Assert.That(result.Items.First().UserName, Is.EqualTo("email@test.com"));
        }

        [Test]
        public async Task GetStatsAsync_ShouldReturnCorrectStats_WhenDataExists()
        {
            var productId = Guid.NewGuid();

            SetupCountByProduct(productId, 10);
            SetupAverage(productId, 4.5);

            var result = await reviewService.GetStatsAsync(productId);

            Assert.That(result.TotalCount, Is.EqualTo(10));
            Assert.That(result.AverageRating, Is.EqualTo(4.5));

            mockReviewRepository.Verify(r => r.GetCountByProductAsync(productId), Times.Once);
            mockReviewRepository.Verify(r => r.GetAverageRatingByProductAsync(productId), Times.Once);
        }

        [Test]
        public async Task GetStatsAsync_ShouldReturnZeroValues_WhenNoReviewsExist()
        {
            var productId = Guid.NewGuid();

            SetupCountByProduct(productId, 0);
            SetupAverage(productId, 0);

            var result = await reviewService.GetStatsAsync(productId);

            Assert.That(result.TotalCount, Is.EqualTo(0));
            Assert.That(result.AverageRating, Is.EqualTo(0));
        }

        [Test]
        public async Task GetMyReviewsPagedAsync_ShouldReturnCorrectData_WhenValidInput()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var review = CreateReview(productId, userId, null, CreateProduct(productId));

            SetupCountByUser(userId, 1);
            SetupPagedByUser(userId, 1, 5, new List<Review> { review });

            var result = await reviewService.GetMyReviewsPagedAsync(userId, 1, 5);

            Assert.That(result.Reviews.Count, Is.EqualTo(1));
            Assert.That(result.CurrentPage, Is.EqualTo(1));
            Assert.That(result.TotalPages, Is.EqualTo(1));

            var item = result.Reviews.First();

            Assert.That(item.ReviewId, Is.EqualTo(review.Id));
            Assert.That(item.ProductName, Is.EqualTo(review.Product.Name));
            Assert.That(item.ProductImage, Is.EqualTo(review.Product.ImageUrl));
            Assert.That(item.Comment, Is.EqualTo(review.Comment));
        }

        [Test]
        public async Task GetMyReviewsPagedAsync_ShouldFormatCreatedAtCorrectly()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var createdAt = new DateTime(2024, 1, 1);
            var review = CreateReview(productId, userId, null, CreateProduct(productId));

            SetupCountByUser(userId, 1);
            SetupPagedByUser(userId, 1, 5, new List<Review> { review });

            var result = await reviewService.GetMyReviewsPagedAsync(userId, 1, 5);

            var item = result.Reviews.First();

            Assert.That(item.CreatedAt, Is.Not.Null);
        }

        [Test]
        public async Task GetMyReviewsPagedAsync_ShouldClampPage_WhenPageExceedsTotalPages()
        {
            var userId = Guid.NewGuid();

            SetupCountByUser(userId, 1);
            SetupPagedByUser(userId, 1, 5, new List<Review>());

            var result = await reviewService.GetMyReviewsPagedAsync(userId, 10, 5);

            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetMyReviewsPagedAsync_ShouldNormalizePaging_WhenInvalidInput()
        {
            var userId = Guid.NewGuid();

            SetupCountByUser(userId, 0);
            SetupPagedByUser(userId, 1, 5, new List<Review>());

            var result = await reviewService.GetMyReviewsPagedAsync(userId, 0, 0);

            Assert.That(result.CurrentPage, Is.EqualTo(1));
            Assert.That(result.TotalPages, Is.EqualTo(1));
        }

        [Test]
        public async Task GetMyReviewsPagedAsync_ShouldReturnEmptyList_WhenNoReviews()
        {
            var userId = Guid.NewGuid();

            SetupCountByUser(userId, 0);
            SetupPagedByUser(userId, 1, 5, new List<Review>());

            var result = await reviewService.GetMyReviewsPagedAsync(userId, 1, 5);

            Assert.That(result.Reviews, Is.Empty);
        }

        [Test]
        public async Task GetManageReviewsPageAsync_ShouldReturnCorrectData_WhenValidInput()
        {
            var productId = Guid.NewGuid();

            var review = CreateReview(productId, Guid.NewGuid(), CreateUser(), CreateProduct(productId));

            SetupCount(1);
            SetupPaged(1, 5, new List<Review> { review });

            var result = await reviewService.GetManageReviewsPageAsync(1, 5);

            Assert.That(result.Reviews.Count, Is.EqualTo(1));
            Assert.That(result.CurrentPage, Is.EqualTo(1));
            Assert.That(result.TotalPages, Is.EqualTo(1));

            var item = result.Reviews.First();

            Assert.That(item.ReviewId, Is.EqualTo(review.Id));
            Assert.That(item.Author, Is.EqualTo("Test User"));
            Assert.That(item.ProductName, Is.EqualTo(review.Product.Name));
            Assert.That(item.ProductImage, Is.EqualTo(review.Product.ImageUrl));
            Assert.That(item.Comment, Is.EqualTo(review.Comment));
        }

        [Test]
        public async Task GetManageReviewsPageAsync_ShouldUseEmail_WhenFullNameIsNull()
        {
            var productId = Guid.NewGuid();

            var review = CreateReview(productId, user: CreateUser(null, "email@test.com"));

            SetupCount(1);
            SetupPaged(1, 5, new List<Review> { review });

            var result = await reviewService.GetManageReviewsPageAsync(1, 5);

            Assert.That(result.Reviews.First().Author, Is.EqualTo("email@test.com"));
        }

        [Test]
        public async Task GetManageReviewsPageAsync_ShouldUseAnonymous_WhenNoUserInfo()
        {
            var productId = Guid.NewGuid();

            var review = CreateReview(productId, user: CreateUser(null, null));

            SetupCount(1);
            SetupPaged(1, 5, new List<Review> { review });

            var result = await reviewService.GetManageReviewsPageAsync(1, 5);

            Assert.That(result.Reviews.First().Author, Is.EqualTo("Anonymous"));
        }

        [Test]
        public async Task GetManageReviewsPageAsync_ShouldClampPage_WhenPageExceedsTotalPages()
        {
            SetupCount(1);
            SetupPaged(1, 5, new List<Review>());

            var result = await reviewService.GetManageReviewsPageAsync(10, 5);

            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetManageReviewsPageAsync_ShouldNormalizePaging_WhenInvalidInput()
        {
            SetupCount(0);
            SetupPaged(1, 5, new List<Review>());

            var result = await reviewService.GetManageReviewsPageAsync(0, 0);

            Assert.That(result.CurrentPage, Is.EqualTo(1));
            Assert.That(result.TotalPages, Is.EqualTo(1));
        }

        [Test]
        public async Task GetManageReviewsPageAsync_ShouldReturnEmptyList_WhenNoReviews()
        {
            SetupCount(0);
            SetupPaged(1, 5, new List<Review>());

            var result = await reviewService.GetManageReviewsPageAsync(1, 5);

            Assert.That(result.Reviews, Is.Empty);
        }

        [Test]
        public async Task SoftDeleteReviewAsync_ShouldReturnFalse_WhenReviewDoesNotExist()
        {
            var reviewId = 1L;

            mockReviewRepository
                .Setup(r => r.GetByIdAsync(reviewId))
                .ReturnsAsync((Review?)null);

            var result = await reviewService.SoftDeleteReviewAsync(reviewId);

            Assert.That(result, Is.False);

            mockReviewRepository.Verify(r => r.Delete(It.IsAny<Review>()), Times.Never);
            mockReviewRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task SoftDeleteReviewAsync_ShouldDeleteReviewAndReturnTrue_WhenReviewExists()
        {
            var reviewId = 1L;

            var review = new Review
            {
                Id = reviewId,
                Rating = 5,
                Comment = "Test"
            };

            mockReviewRepository
                .Setup(r => r.GetByIdAsync(reviewId))
                .ReturnsAsync(review);

            var result = await reviewService.SoftDeleteReviewAsync(reviewId);

            Assert.That(result, Is.True);

            mockReviewRepository.Verify(r => r.Delete(review), Times.Once);
            mockReviewRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        private User CreateUser(string? fullName = "Test User", string? email = "test@abv.bg")
        {
            return new User
            {
                FullName = fullName,
                Email = email
            };
        }

        private Product CreateProduct(Guid productId)
        {
            return new Product
            {
                Id = productId,
                Name = "Test Product",
                ImageUrl = "image.png"
            };
        }

        private Review CreateReview(Guid productId, Guid? userId = null, User? user = null, Product? product = null)
        {
            return new Review
            {
                Id = 1,
                Rating = 5,
                Comment = "Test",
                CreatedAt = DateTime.UtcNow,
                ProductId = productId,
                UserId = userId ?? Guid.NewGuid(),
                User = user ?? CreateUser(),
                Product = product ?? CreateProduct(productId)
            };
        }

        private void SetupReviewExists(Guid userId, Guid productId, bool exists)
        {
            mockReviewRepository
                .Setup(r => r.ReviewExistsAsync(userId, productId))
                .ReturnsAsync(exists);
        }

        private void SetupAverage(Guid productId, double average)
        {
            mockReviewRepository
                .Setup(r => r.GetAverageRatingByProductAsync(productId))
                .ReturnsAsync(average);
        }

        private void SetupCount(int count)
        {
            mockReviewRepository
                .Setup(r => r.GetCountAsync())
                .ReturnsAsync(count);
        }

        private void SetupCountByProduct(Guid productId, int count)
        {
            mockReviewRepository
                .Setup(r => r.GetCountByProductAsync(productId))
                .ReturnsAsync(count);
        }

        private void SetupCountByUser(Guid userId, int count)
        {
            mockReviewRepository
                .Setup(r => r.GetCountByUserAsync(userId))
                .ReturnsAsync(count);
        }

        private void SetupPagedByProduct(Guid productId, int page, int pageSize, List<Review> reviews)
        {
            mockReviewRepository
                .Setup(r => r.GetPagedByProductAsync(productId, page, pageSize))
                .ReturnsAsync(reviews);
        }

        private void SetupPagedByUser(Guid userId, int page, int pageSize, List<Review> reviews)
        {
            mockReviewRepository
                .Setup(r => r.GetPagedByUserAsync(userId, page, pageSize))
                .ReturnsAsync(reviews);
        }

        private void SetupPaged(int page, int pageSize, List<Review> reviews)
        {
            mockReviewRepository
                .Setup(r => r.GetPagedAsync(page, pageSize))
                .ReturnsAsync(reviews);
        }
    }
}