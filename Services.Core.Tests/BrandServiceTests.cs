using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.GCommon;
using TechStore.Web.ViewModels.Brand;

namespace TechStore.Services.Core.Tests
{
    [TestFixture]
    public class BrandServiceTests
    {
        private Mock<IBrandRepository> mockBrandRepository = null!;
        private BrandService brandService = null!;

        private const string TestLogo = "https://example.com/logo.png";
        private const string TestDescription = "Test Description";

        [SetUp]
        public void SetUp()
        {
            this.mockBrandRepository = new Mock<IBrandRepository>();
            this.brandService = new BrandService(mockBrandRepository.Object);
        }

        [Test]
        public void PassAlways()
        {
            // Test that will always pass to show that the SetUp is working
            Assert.Pass();
        }

        [Test]
        public async Task GetBrandDetailsViewModelAsync_ShouldReturnNull_WhenIdIsNull()
        {
            var result = await brandService.GetBrandDetailsViewModelAsync(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetBrandDetailsViewModelAsync_ShouldReturnNull_WhenBrandDoesNotExist()
        {
            var brandList = new List<Brand>();
            var mockBrands = brandList.AsQueryable().BuildMockDbSet();

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockBrands.Object);

            var result = await brandService.GetBrandDetailsViewModelAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetBrandDetailsViewModelAsync_ShouldReturnCorrectModel_WhenBrandExists()
        {
            var testBrand = new Brand
            {
                Id = 1,
                Name = "TestBrand",
                Description = TestDescription,
                LogoUrl = TestLogo
            };

            var mockBrands = MockHelper.CreateMockQueryable(new List<Brand> { testBrand });

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockBrands);

            var result = await brandService.GetBrandDetailsViewModelAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(testBrand.Id));
            Assert.That(result.Name, Is.EqualTo(testBrand.Name));
            Assert.That(result.Description, Is.EqualTo(testBrand.Description));
            Assert.That(result.logoUrl, Is.EqualTo(testBrand.LogoUrl));
        }

        [Test]
        public async Task GetBrandsDropDownDataAsync_ShouldReturnMappedModels_WhenBrandsExist()
        {
            var brands = new List<Brand>
            {
                new Brand { Id = 1, Name = "Brand A" },
                new Brand { Id = 2, Name = "Brand B" }
            };

            var mockQueryable = MockHelper.CreateMockQueryable(brands);

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = (await brandService.GetBrandsDropDownDataAsync()).ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Brand A"));
            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[1].Name, Is.EqualTo("Brand B"));
        }

        [Test]
        public async Task GetBrandsDropDownDataAsync_ShouldReturnEmptyList_WhenNoBrandsExist()
        {
            var brands = new List<Brand>();

            var mockQueryable = MockHelper.CreateMockQueryable(brands);

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandsDropDownDataAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task AddBrandAsync_ShouldReturnFalse_WhenNameIsNullOrWhiteSpace()
        {
            var inputModel = new BrandFormInputViewModel
            {
                Name = "   ",
                LogoUrl = TestLogo,
                Description = TestDescription
            };

            var result = await brandService.AddBrandAsync(inputModel);

            Assert.That(result, Is.False);

            mockBrandRepository.Verify(r => r.AddAsync(It.IsAny<Brand>()), Times.Never);
            mockBrandRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task AddBrandAsync_ShouldAddBrandAndReturnTrue_WhenValidInput()
        {
            Brand? addedBrand = null;

            mockBrandRepository
                .Setup(r => r.AddAsync(It.IsAny<Brand>()))
                .Callback<Brand>(b => addedBrand = b);

            var inputModel = new BrandFormInputViewModel
            {
                Name = " Tesla ",
                LogoUrl = TestLogo,
                Description = TestDescription
            };

            var result = await brandService.AddBrandAsync(inputModel);

            Assert.That(result, Is.True);
            Assert.That(addedBrand, Is.Not.Null);
            Assert.That(addedBrand!.Name, Is.EqualTo("Tesla")); //Trimmed
            Assert.That(addedBrand.LogoUrl, Is.EqualTo(inputModel.LogoUrl));
            Assert.That(addedBrand.Description, Is.EqualTo(inputModel.Description));

            mockBrandRepository.Verify(r => r.AddAsync(It.IsAny<Brand>()), Times.Once);
            mockBrandRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task AddBrandAsync_ShouldUseDefaultImageUrl_WhenLogoUrlIsNull()
        {
            Brand? addedBrand = null;

            mockBrandRepository
                .Setup(r => r.AddAsync(It.IsAny<Brand>()))
                .Callback<Brand>(b => addedBrand = b);

            var inputModel = new BrandFormInputViewModel
            {
                Name = "Tesla",
                LogoUrl = null,
                Description = TestDescription
            };

            var result = await brandService.AddBrandAsync(inputModel);

            Assert.That(result, Is.True);
            Assert.That(addedBrand, Is.Not.Null);
            Assert.That(addedBrand!.Name, Is.EqualTo(inputModel.Name));
            Assert.That(addedBrand.LogoUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));
            Assert.That(addedBrand.Description, Is.EqualTo(inputModel.Description));

            mockBrandRepository.Verify(r => r.AddAsync(It.IsAny<Brand>()), Times.Once);
            mockBrandRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetEditableBrandByIdAsync_ShouldReturnNull_WhenBrandIdIsNull()
        {
            var result = await brandService.GetEditableBrandByIdAsync(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditableBrandByIdAsync_ShouldReturnNull_WhenBrandIdIsLessThanOrEqualToZero()
        {
            var result = await brandService.GetEditableBrandByIdAsync(0);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditableBrandByIdAsync_ShouldReturnNull_WhenBrandDoesNotExist()
        {
            var brands = new List<Brand>();
            var mockQueryable = MockHelper.CreateMockQueryable(brands);

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetEditableBrandByIdAsync(1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditableBrandByIdAsync_ShouldReturnBrand_WhenBrandExists()
        {
            var brand = new Brand
            {
                Id = 1,
                Name = "Test Brand",
                LogoUrl = TestLogo,
                Description = TestDescription
            };

            var mockQueryable = MockHelper.CreateMockQueryable(new List<Brand> { brand });

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetEditableBrandByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(brand.Id));
            Assert.That(result.Name, Is.EqualTo(brand.Name));
            Assert.That(result.LogoUrl, Is.EqualTo(brand.LogoUrl));
            Assert.That(result.Description, Is.EqualTo(brand.Description));
        }

        [Test]
        public async Task GetEditableBrandByIdAsync_ShouldReturnDefaultImage_WhenLogoUrlIsNull()
        {
            var brand = new Brand
            {
                Id = 1,
                Name = "Test Brand",
                LogoUrl = null,
                Description = TestDescription
            };

            var mockQueryable = MockHelper.CreateMockQueryable(new List<Brand> { brand });

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetEditableBrandByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.LogoUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));
        }

        [Test]
        public async Task EditBrandAsync_ShouldReturnFalse_WhenBrandDoesNotExist()
        {
            var inputModel = new BrandFormInputViewModel
            {
                Id = 1,
                Name = "Tesla",
                LogoUrl = TestLogo,
                Description = TestDescription
            };

            mockBrandRepository
                .Setup(r => r.GetByIdAsync(inputModel.Id))
                .ReturnsAsync((Brand?)null);

            var result = await brandService.EditBrandAsync(inputModel);

            Assert.That(result, Is.False);

            mockBrandRepository.Verify(r => r.Update(It.IsAny<Brand>()), Times.Never);
            mockBrandRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task EditBrandAsync_ShouldUpdateBrandAndReturnTrue_WhenValidInput()
        {
            var existingBrand = new Brand
            {
                Id = 1,
                Name = "OldName",
                LogoUrl = "old.png",
                Description = "old description"
            };

            Brand? updatedBrand = null;

            mockBrandRepository
                .Setup(r => r.GetByIdAsync(existingBrand.Id))
                .ReturnsAsync(existingBrand);

            mockBrandRepository
                .Setup(r => r.Update(It.IsAny<Brand>()))
                .Callback<Brand>(b => updatedBrand = b);

            var inputModel = new BrandFormInputViewModel
            {
                Id = existingBrand.Id,
                Name = " Tesla ",
                LogoUrl = TestLogo,
                Description = TestDescription
            };

            var result = await brandService.EditBrandAsync(inputModel);

            Assert.That(result, Is.True);
            Assert.That(updatedBrand, Is.Not.Null);
            Assert.That(updatedBrand!.Name, Is.EqualTo("Tesla")); // Trimmed
            Assert.That(updatedBrand.LogoUrl, Is.EqualTo(inputModel.LogoUrl));
            Assert.That(updatedBrand.Description, Is.EqualTo(inputModel.Description));

            mockBrandRepository.Verify(r => r.Update(It.IsAny<Brand>()), Times.Once);
            mockBrandRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task EditBrandAsync_ShouldUseDefaultImageUrl_WhenLogoUrlIsNull()
        {
            var existingBrand = new Brand
            {
                Id = 1,
                Name = "OldName",
                LogoUrl = "old.png",
                Description = "old description"
            };

            Brand? updatedBrand = null;

            mockBrandRepository
                .Setup(r => r.GetByIdAsync(existingBrand.Id))
                .ReturnsAsync(existingBrand);

            mockBrandRepository
                .Setup(r => r.Update(It.IsAny<Brand>()))
                .Callback<Brand>(b => updatedBrand = b);

            var inputModel = new BrandFormInputViewModel
            {
                Id = existingBrand.Id,
                Name = "Tesla",
                LogoUrl = null,
                Description = TestDescription
            };

            var result = await brandService.EditBrandAsync(inputModel);

            Assert.That(result, Is.True);
            Assert.That(updatedBrand, Is.Not.Null);
            Assert.That(updatedBrand!.LogoUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));

            mockBrandRepository.Verify(r => r.Update(It.IsAny<Brand>()), Times.Once);
            mockBrandRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetBrandForDeleteByIdAsync_ShouldReturnNull_WhenBrandIdIsNull()
        {
            var result = await brandService.GetBrandForDeleteByIdAsync(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetBrandForDeleteByIdAsync_ShouldReturnNull_WhenBrandDoesNotExist()
        {
            var brands = new List<Brand>();
            var mockQueryable = MockHelper.CreateMockQueryable(brands);

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandForDeleteByIdAsync(10);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetBrandForDeleteByIdAsync_ShouldReturnNull_WhenBrandIsDeleted()
        {
            Brand brand = new Brand
            {
                Id = 1,
                Name = "Deleted Brand",
                IsDeleted = true
            };
            
            var mockQueryable = MockHelper.CreateMockQueryable(new List<Brand> { brand });

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandForDeleteByIdAsync(1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetBrandForDeleteByIdAsync_ShouldReturnBrandViewModel_WhenBrandExistsAndIsNotDeleted()
        {
            var brand = new Brand
            {
                Id = 1,
                Name = "Valid Brand",
                Description = TestDescription,
                LogoUrl = TestLogo,
                IsDeleted = false
            };

            var mockQueryable = MockHelper.CreateMockQueryable(new List<Brand> { brand });

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandForDeleteByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(brand.Id));
            Assert.That(result.Name, Is.EqualTo(brand.Name));
            Assert.That(result.Description, Is.EqualTo(brand.Description));
            Assert.That(result.LogoUrl, Is.EqualTo(brand.LogoUrl));
        }

        [Test]
        public async Task SoftDeleteBrandAsync_ShouldReturnFalse_WhenBrandDoesNotExist()
        {
            mockBrandRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Brand?)null);

            var deleteModel = new DeleteBrandViewModel { Id = 1 };

            var result = await brandService.SoftDeleteBrandAsync(deleteModel);

            Assert.That(result, Is.False);

            mockBrandRepository.Verify(r => r.Delete(It.IsAny<Brand>()), Times.Never);
            mockBrandRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task SoftDeleteBrandAsync_ShouldDeleteBrandAndReturnTrue_WhenBrandExists()
        {
            var brand = new Brand
            {
                Id = 1,
                Name = "Test Brand",
                Description = TestDescription,
                LogoUrl = TestLogo,
                IsDeleted = false
            };

            mockBrandRepository
                .Setup(r => r.GetByIdAsync(brand.Id))
                .ReturnsAsync(brand);

            var deleteModel = new DeleteBrandViewModel
            {
                Id = brand.Id
            };

            var result = await brandService.SoftDeleteBrandAsync(deleteModel);

            Assert.That(result, Is.True);

            mockBrandRepository.Verify(r => r.Delete(brand), Times.Once);
            mockBrandRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetBrandForRestoreByIdAsync_ShouldReturnNull_WhenBrandDoesNotExist()
        {
            var brands = new List<Brand>();
            var mockQueryable = MockHelper.CreateMockQueryable(brands);

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandForRestoreByIdAsync(10);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetBrandForRestoreByIdAsync_ShouldReturnBrandViewModel_WhenBrandExistsAndIsDeleted()
        {
            Brand brand = new Brand
            {
                Id = 1,
                Name = "Test Brand",
                Description = TestDescription,
                LogoUrl = TestLogo,
                IsDeleted = true
            };

            var mockQueryable = MockHelper.CreateMockQueryable(new List<Brand> { brand });

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandForRestoreByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(brand.Id));
            Assert.That(result.Name, Is.EqualTo(brand.Name));
            Assert.That(result.Description, Is.EqualTo(brand.Description));
            Assert.That(result.LogoUrl, Is.EqualTo(brand.LogoUrl));
        }

        [Test]
        public async Task RestoreByIdAsync_ShouldReturnFalse_WhenBrandDoesNotExist()
        {
            var brands = new List<Brand>();
            var mockQueryable = MockHelper.CreateMockQueryable(brands);

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.RestoreByIdAsync(1);

            Assert.That(result, Is.False);

            mockBrandRepository.Verify(r => r.Update(It.IsAny<Brand>()), Times.Never);
            mockBrandRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task RestoreByIdAsync_ShouldRestoreBrandAndReturnTrue_WhenBrandExists()
        {
            Brand brand = new Brand
            {
                Id = 1,
                Name = "Test Brand",
                Description = TestDescription,
                LogoUrl = TestLogo,
                IsDeleted = true
            };

            var mockQueryable = MockHelper.CreateMockQueryable(new List<Brand>() { brand });

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.RestoreByIdAsync(1);

            Assert.That(result, Is.True);
            Assert.That(brand.IsDeleted, Is.False);

            mockBrandRepository.Verify(r => r.Update(It.IsAny<Brand>()), Times.Once);
            mockBrandRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ExistsByNameAsync_ShouldReturnTrue_WhenBrandExists()
        {
            string name = "Tesla";
            int skipId = 1;

            mockBrandRepository
                .Setup(r => r.ExistsByNameAsync(name, skipId))
                .ReturnsAsync(true);

            var result = await brandService.ExistsByNameAsync(name, skipId);

            Assert.That(result, Is.True);

            mockBrandRepository.Verify(r => r.ExistsByNameAsync(name, skipId), Times.Once);
        }

        [Test]
        public async Task ExistsByNameAsync_ShouldReturnFalse_WhenBrandDoesNotExist()
        {
            string name = "NonExistingBrand";
            int skipId = 2;

            mockBrandRepository
                .Setup(r => r.ExistsByNameAsync(name, skipId))
                .ReturnsAsync(false);

            var result = await brandService.ExistsByNameAsync(name, skipId);

            Assert.That(result, Is.False);

            mockBrandRepository.Verify(r => r.ExistsByNameAsync(name, skipId), Times.Once);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrue_WhenBrandExists()
        {
            int id = 1;

            mockBrandRepository
                .Setup(r => r.ExistsAsync(id))
                .ReturnsAsync(true);

            var result = await brandService.ExistsAsync(id);

            Assert.That(result, Is.True);

            mockBrandRepository.Verify(r => r.ExistsAsync(id), Times.Once);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalse_WhenBrandDoesNotExist()
        {
            int id = 2;

            mockBrandRepository
                .Setup(r => r.ExistsAsync(id))
                .ReturnsAsync(false);

            var result = await brandService.ExistsAsync(id);

            Assert.That(result, Is.False);

            mockBrandRepository.Verify(r => r.ExistsAsync(id), Times.Once);
        }


        [Test]
        public async Task GetDeletedBrandByNameAsync_ShouldReturnBrand_WhenDeletedBrandExists()
        {
            string name = "Deleted Brand";
            var deletedBrand = new Brand {
                Id = 5, 
                Name = name, 
                IsDeleted = true 
            };

            mockBrandRepository
                .Setup(r => r.GetDeletedBrandByNameAsync(name))
                .ReturnsAsync(deletedBrand);

            var result = await brandService.GetDeletedBrandByNameAsync(name);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo(name));

            mockBrandRepository.Verify(r => r.GetDeletedBrandByNameAsync(name), Times.Once);
        }

        [Test]
        public async Task GetDeletedBrandByNameAsync_ShouldReturnNull_WhenDeletedBrandDoesNotExist()
        {
            string name = "NonDeletedBrand";

            mockBrandRepository
                .Setup(r => r.GetDeletedBrandByNameAsync(name))
                .ReturnsAsync((Brand?)null);

            var result = await brandService.GetDeletedBrandByNameAsync(name);

            Assert.That(result, Is.Null);

            mockBrandRepository.Verify(r => r.GetDeletedBrandByNameAsync(name), Times.Once);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenNoBrandsAvailable()
        {
            var brands = new List<Brand>();

            var mockQueryable = MockHelper.CreateMockQueryable(brands);

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnOrderedBrandsWithCorrectMapping()
        {
            var brand1 = new Brand()
            {
                Id = 1,
                Name = "Tesla2",
                Description = TestDescription,
                LogoUrl = TestLogo
            };

            var brand2 = new Brand()
            {
                Id = 2,
                Name = "Tesla1",
                Description = TestDescription,
                LogoUrl = null
            };

            var brands = new List<Brand> { brand1, brand2 };

            var mockQueryable = MockHelper.CreateMockQueryable(brands);

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = (await brandService.GetAllAsync()).ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(brand2.Id));  //Correct ordering
            Assert.That(result[1].Id, Is.EqualTo(brand1.Id));
            Assert.That(result[0].Name, Is.EqualTo(brand2.Name));
            Assert.That(result[1].Name, Is.EqualTo(brand1.Name));
            Assert.That(result[0].LogoUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));  //Correct use of DefaultImageUrl
            Assert.That(result[1].LogoUrl, Is.EqualTo(brand1.LogoUrl));
            Assert.That(result[0].Description, Is.EqualTo(brand2.Description));
            Assert.That(result[1].Description, Is.EqualTo(brand1.Description));
        }
    }
}
