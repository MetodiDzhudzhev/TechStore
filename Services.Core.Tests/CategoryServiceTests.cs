using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.GCommon;
using TechStore.Web.ViewModels.Category;

namespace TechStore.Services.Core.Tests
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<ICategoryRepository> mockCategoryRepository = null!;
        private CategoryService categoryService = null!;

        private const string TestName = "Phones";
        private const string TestImageUrl = "https://example.com/logo.png";

        [SetUp]
        public void SetUp()
        {
            this.mockCategoryRepository = new Mock<ICategoryRepository>();
            this.categoryService = new CategoryService(mockCategoryRepository.Object);
        }

        [Test]
        public void PassAlways()
        {
            // Test that will always pass to show that the SetUp is working
            Assert.Pass();
        }

        [Test]
        public async Task GetAllCategoriesAsync_ShouldReturnEmptyCollection_WhenNoCategories()
        {
            var categories = new List<Category>();

            var mockQueryable = CreateMockQueryable(categories);

            SetupCategories(mockQueryable);

            var result = await categoryService.GetAllCategoriesAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllCategoriesAsync_ShouldMapCategoriesCorrectly()
        {
            var category1 = CreateCategory(1, TestName, TestImageUrl);
            var category2 = CreateCategory(2, "Laptops", null);

            var categories = new List<Category> { category1, category2 };

            var mockQueryable = CreateMockQueryable(categories);

            SetupCategories(mockQueryable);

            var result = (await categoryService.GetAllCategoriesAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result[0].Id, Is.EqualTo(category1.Id));
            Assert.That(result[0].Name, Is.EqualTo(category1.Name));
            Assert.That(result[0].ImageUrl, Is.EqualTo(category1.ImageUrl));

            Assert.That(result[1].Id, Is.EqualTo(category2.Id));
            Assert.That(result[1].Name, Is.EqualTo(category2.Name));
            Assert.That(result[1].ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));
        }

        [Test]
        public async Task AddCategoryAsync_ShouldAddCategory_WithCorrectData()
        {
            var inputModel = new CategoryFormInputViewModel
            {
                Name = " Phones ",
                ImageUrl = TestImageUrl,
            };

            Category? addedCategory = null;

            mockCategoryRepository
                .Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Callback<Category>(c => addedCategory = c);

            await categoryService.AddCategoryAsync(inputModel);

            Assert.That(addedCategory, Is.Not.Null);
            Assert.That(addedCategory!.Name, Is.EqualTo("Phones")); //Тrimmed
            Assert.That(addedCategory.ImageUrl, Is.EqualTo(inputModel.ImageUrl));

            mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task AddCategoryAsync_ShouldUseDefaultImage_WhenImageUrlIsNull()
        {
            var inputModel = new CategoryFormInputViewModel
            {
                Name = TestName,
                ImageUrl = null
            };

            Category? addedCategory = null;

            mockCategoryRepository
                .Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Callback<Category>(c => addedCategory = c);

            await categoryService.AddCategoryAsync(inputModel);

            Assert.That(addedCategory, Is.Not.Null);
            Assert.That(addedCategory!.ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));

            mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetEditableCategoryByIdAsync_ShouldReturnNull_WhenIdIsNull()
        {
            var result = await categoryService.GetEditableCategoryByIdAsync(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditableCategoryByIdAsync_ShouldReturnNull_WhenIdIsLessThanOrEqualToZero()
        {
            var result = await categoryService.GetEditableCategoryByIdAsync(0);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditableCategoryByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            var categories = new List<Category>();

            var mockQueryable = CreateMockQueryable(categories);

            SetupCategories(mockQueryable);

            var result = await categoryService.GetEditableCategoryByIdAsync(1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditableCategoryByIdAsync_ShouldReturnCorrectModel_WhenCategoryExists()
        {
            var category = CreateCategory(1, TestName, TestImageUrl);

            var mockQueryable = CreateMockQueryable(new List<Category> { category });

            SetupCategories(mockQueryable);

            var result = await categoryService.GetEditableCategoryByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(category.Id));
            Assert.That(result.Name, Is.EqualTo(category.Name));
            Assert.That(result.ImageUrl, Is.EqualTo(category.ImageUrl));
        }

        [Test]
        public async Task GetEditableCategoryByIdAsync_ShouldUseDefaultImage_WhenImageUrlIsNull()
        {
            var category = CreateCategory(1, TestName, null);

            var mockQueryable = CreateMockQueryable(new List<Category> { category });

            SetupCategories(mockQueryable);

            var result = await categoryService.GetEditableCategoryByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));
        }

        [Test]
        public async Task EditCategoryAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            var inputModel = new CategoryFormInputViewModel
            {
                Id = 1,
                Name = TestName,
                ImageUrl = TestImageUrl
            };

            SetupCategoryById(inputModel.Id, null);

            var result = await categoryService.EditCategoryAsync(inputModel);

            Assert.That(result, Is.False);

            mockCategoryRepository.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
            mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task EditCategoryAsync_ShouldUpdateCategoryAndReturnTrue_WhenCategoryExists()
        {
            var category = CreateCategory(1, TestName, TestImageUrl);

            var inputModel = new CategoryFormInputViewModel
            {
                Id = category.Id,
                Name = " NewName ",
                ImageUrl = "new.png"
            };

            SetupCategoryById(category.Id, category);

            var result = await categoryService.EditCategoryAsync(inputModel);

            Assert.That(result, Is.True);

            Assert.That(category.Name, Is.EqualTo("NewName")); //Trimmed
            Assert.That(category.ImageUrl, Is.EqualTo(inputModel.ImageUrl));

            mockCategoryRepository.Verify(r => r.Update(category), Times.Once);
            mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task EditCategoryAsync_ShouldUseDefaultImage_WhenImageUrlIsNull()
        {
            var category = CreateCategory(1, TestName, TestImageUrl);

            var inputModel = new CategoryFormInputViewModel
            {
                Id = category.Id,
                Name = "NewName",
                ImageUrl = null
            };

            SetupCategoryById(category.Id, category);

            var result = await categoryService.EditCategoryAsync(inputModel);

            Assert.That(result, Is.True);
            Assert.That(category.ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));

            mockCategoryRepository.Verify(r => r.Update(category), Times.Once);
            mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetCategoryForDeleteByIdAsync_ShouldReturnNull_WhenIdIsNull()
        {
            var result = await categoryService.GetCategoryForDeleteByIdAsync(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCategoryForDeleteByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            var categories = new List<Category>();

            var mockQueryable = CreateMockQueryable(categories);

            SetupCategories(mockQueryable);

            var result = await categoryService.GetCategoryForDeleteByIdAsync(1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCategoryForDeleteByIdAsync_ShouldReturnNull_WhenCategoryIsDeleted()
        {
            var category = CreateCategory(1, TestName, TestImageUrl, isDeleted: true);

            var mockQueryable = CreateMockQueryable(new List<Category> { category });

            SetupCategories(mockQueryable);

            var result = await categoryService.GetCategoryForDeleteByIdAsync(1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCategoryForDeleteByIdAsync_ShouldReturnCorrectModel_WhenValid()
        {
            var category = CreateCategory(1, TestName, TestImageUrl, isDeleted: false);

            var mockQueryable = CreateMockQueryable(new List<Category> { category });

            SetupCategories(mockQueryable);

            var result = await categoryService.GetCategoryForDeleteByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(category.Id));
            Assert.That(result.Name, Is.EqualTo(category.Name));
            Assert.That(result.ImageUrl, Is.EqualTo(category.ImageUrl));
        }

        [Test]
        public async Task SoftDeleteCategoryAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            var deleteModel = new DeleteCategoryViewModel
            {
                Id = 1
            };

            SetupCategoryById(1, null);

            var result = await categoryService.SoftDeleteCategoryAsync(deleteModel);

            Assert.That(result, Is.False);

            mockCategoryRepository.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
            mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task SoftDeleteCategoryAsync_ShouldDeleteCategoryAndReturnTrue_WhenCategoryExists()
        {
            var category = CreateCategory(1, TestName, TestImageUrl);

            var deleteModel = new DeleteCategoryViewModel
            {
                Id = category.Id
            };

            SetupCategoryById(category.Id, category);

            var result = await categoryService.SoftDeleteCategoryAsync(deleteModel);

            Assert.That(result, Is.True);

            mockCategoryRepository.Verify(r => r.Delete(category), Times.Once);
            mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetCategoriesDropDownDataAsync_ShouldReturnEmptyCollection_WhenNoCategories()
        {
            var categories = new List<Category>();

            var mockQueryable = CreateMockQueryable(categories);
            SetupCategories(mockQueryable);

            var result = await categoryService.GetCategoriesDropDownDataAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetCategoriesDropDownDataAsync_ShouldMapCategoriesCorrectly()
        {
            var category1 = CreateCategory(1, TestName, TestImageUrl);
            var category2 = CreateCategory(2, "Laptops", null);

            var categories = new List<Category> { category1, category2 };

            var mockQueryable = CreateMockQueryable(categories);
            SetupCategories(mockQueryable);

            var result = (await categoryService.GetCategoriesDropDownDataAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result[0].Id, Is.EqualTo(category1.Id));
            Assert.That(result[0].Name, Is.EqualTo(category1.Name));

            Assert.That(result[1].Id, Is.EqualTo(category2.Id));
            Assert.That(result[1].Name, Is.EqualTo(category2.Name));
        }

        [Test]
        public async Task ExistsByNameAsync_ShouldReturnTrue_WhenCategoryExists()
        {
            var skipId = 1;

            mockCategoryRepository
                .Setup(r => r.ExistsByNameAsync(TestName, skipId))
                .ReturnsAsync(true);

            var result = await categoryService.ExistsByNameAsync(TestName, skipId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsByNameAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            var skipId = 2;

            mockCategoryRepository
                .Setup(r => r.ExistsByNameAsync(TestName, skipId))
                .ReturnsAsync(false);

            var result = await categoryService.ExistsByNameAsync(TestName, skipId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrue_WhenCategoryExists()
        {
            var id = 1;

            mockCategoryRepository
                .Setup(r => r.ExistsAsync(id))
                .ReturnsAsync(true);

            var result = await categoryService.ExistsAsync(id);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            var id = 2;

            mockCategoryRepository
                .Setup(r => r.ExistsAsync(id))
                .ReturnsAsync(false);

            var result = await categoryService.ExistsAsync(id);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetDeletedCategoryByNameAsync_ShouldReturnCategory_WhenDeletedCategoryExists()
        {
            var category = CreateCategory(5, TestName, TestImageUrl, isDeleted: true);

            mockCategoryRepository
                .Setup(r => r.GetDeletedCategoryByNameAsync(TestName))
                .ReturnsAsync(category);

            var result = await categoryService.GetDeletedCategoryByNameAsync(TestName);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo(TestName));
            Assert.That(result.IsDeleted, Is.True);

            mockCategoryRepository.Verify(r => r.GetDeletedCategoryByNameAsync(TestName), Times.Once);
        }

        [Test]
        public async Task GetDeletedCategoryByNameAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            mockCategoryRepository
                .Setup(r => r.GetDeletedCategoryByNameAsync(TestName))
                .ReturnsAsync((Category?)null);

            var result = await categoryService.GetDeletedCategoryByNameAsync(TestName);

            Assert.That(result, Is.Null);

            mockCategoryRepository.Verify(r => r.GetDeletedCategoryByNameAsync(TestName), Times.Once);
        }

        [Test]
        public async Task GetCategoryForRestoreByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            var categories = new List<Category>();

            var mockQueryable = CreateMockQueryable(categories);
            SetupCategories(mockQueryable);

            var result = await categoryService.GetCategoryForRestoreByIdAsync(1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCategoryForRestoreByIdAsync_ShouldReturnCategory_WhenCategoryExists()
        {
            var category = CreateCategory(1, TestName, TestImageUrl, isDeleted: true);

            var categories = new List<Category> { category };

            var mockQueryable = CreateMockQueryable(categories);
            SetupCategories(mockQueryable);

            var result = await categoryService.GetCategoryForRestoreByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(category.Id));
            Assert.That(result.Name, Is.EqualTo(category.Name));
            Assert.That(result.ImageUrl, Is.EqualTo(category.ImageUrl));
        }

        [Test]
        public async Task GetCategoryForRestoreByIdAsync_ShouldUseDefaultImage_WhenImageUrlIsNull()
        {
            var category = CreateCategory(1, TestName, null);

            var categories = new List<Category> { category };

            var mockQueryable = CreateMockQueryable(categories);
            SetupCategories(mockQueryable);

            var result = await categoryService.GetCategoryForRestoreByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(category.Id));
            Assert.That(result.Name, Is.EqualTo(category.Name));
            Assert.That(result!.ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));
        }

        [Test]
        public async Task RestoreByIdAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            var categories = new List<Category>();

            var mockQueryable = CreateMockQueryable(categories);
            SetupCategories(mockQueryable);

            var result = await categoryService.RestoreByIdAsync(1);

            Assert.That(result, Is.False);

            mockCategoryRepository.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
            mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task RestoreByIdAsync_ShouldRestoreCategoryAndReturnTrue_WhenCategoryExists()
        {
            var category = CreateCategory(1, TestName, TestImageUrl, isDeleted: true);

            var categories = new List<Category> { category };

            var mockQueryable = CreateMockQueryable(categories);
            SetupCategories(mockQueryable);

            var result = await categoryService.RestoreByIdAsync(1);

            Assert.That(result, Is.True);
            Assert.That(category.IsDeleted, Is.False);

            mockCategoryRepository.Verify(r => r.Update(category), Times.Once);
            mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenNoCategories()
        {
            var categories = new List<Category>();

            var mockQueryable = CreateMockQueryable(categories);
            SetupCategories(mockQueryable);

            var result = await categoryService.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnOrderedCategoriesWithCorrectMapping()
        {
            var category1 = CreateCategory(1, TestName, TestImageUrl);
            var category2 = CreateCategory(2, "Accessories", null);

            var categories = new List<Category> { category1, category2 };

            var mockQueryable = CreateMockQueryable(categories);
            SetupCategories(mockQueryable);

            var result = (await categoryService.GetAllAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result[0].Id, Is.EqualTo(category2.Id));  //Correct ordering
            Assert.That(result[1].Id, Is.EqualTo(category1.Id));
            Assert.That(result[0].Name, Is.EqualTo(category2.Name));
            Assert.That(result[1].Name, Is.EqualTo(category1.Name));


            Assert.That(result[0].ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));  //Correct use of DefaultImageUrl
            Assert.That(result[1].ImageUrl, Is.EqualTo(category1.ImageUrl));
        }

        private void SetupCategories(IQueryable<Category> categories)
        {
            mockCategoryRepository
                .Setup(r => r.GetAllAttached())
                .Returns(categories);
        }

        private void SetupCategoryById(int id, Category? category)
        {
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(category);
        }

        private IQueryable<Category> CreateMockQueryable(List<Category> data)
        {
            return data.AsQueryable().BuildMockDbSet().Object;
        }

        private Category CreateCategory(int id, string name, string? imageUrl = null, bool isDeleted = false)
        {
            return new Category
            {
                Id = id,
                Name = name,
                ImageUrl = imageUrl,
                IsDeleted = isDeleted
            };
        }
    }
}