using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Web.ViewModels.Brand;

namespace TechStore.Services.Core.Tests
{
    [TestFixture]
    public class BrandServiceTests
    {
        private Mock<IBrandRepository> mockBrandRepository;
        private Mock<IUserRepository> mockUserRepository;
        private BrandService brandService;

        private const string DefaultImageUrl = "/images/NoImage.jpg";

        [SetUp]
        public void SetUp()
        {
            this.mockBrandRepository = new Mock<IBrandRepository>();
            this.mockUserRepository = new Mock<IUserRepository>();
            this.brandService = new BrandService(mockBrandRepository.Object, mockUserRepository.Object);
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
                Description = "Test Description",
                LogoUrl = "https://example.com/logo.png"
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
        public async Task AddBrandAsync_ShouldReturnFalse_IfUserDoesNotExist()
        {
            var fakeUserId = Guid.NewGuid().ToString();

            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            var input = new BrandFormInputViewModel
            {
                Name = "Test Brand",
                LogoUrl = "someurl.jpg",
                Description = "Some description"
            };

            var result = await brandService.AddBrandAsync(fakeUserId, input);

            Assert.That(result, Is.False);
            mockBrandRepository.Verify(r => r.AddAsync(It.IsAny<Brand>()), Times.Never);
        }

        [Test]
        public async Task AddBrandAsync_ShouldAddBrandAndReturnTrue_WhenUserExists()
        {
            var userId = Guid.NewGuid().ToString();

            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            Brand? addedBrand = null;
            mockBrandRepository
                .Setup(r => r.AddAsync(It.IsAny<Brand>()))
                .Callback<Brand>(b => addedBrand = b)
                .Returns(Task.CompletedTask);

            var input = new BrandFormInputViewModel
            {
                Name = "  Test Brand  ", // Test Trim()
                LogoUrl = "logo.jpg",
                Description = "Description text"
            };

            var result = await brandService.AddBrandAsync(userId, input);

            Assert.That(result, Is.True);
            Assert.That(addedBrand, Is.Not.Null);
            Assert.That(addedBrand!.Name, Is.EqualTo("Test Brand"));  //Trimmed
            Assert.That(addedBrand.LogoUrl, Is.EqualTo("logo.jpg"));
            Assert.That(addedBrand.Description, Is.EqualTo("Description text"));

            mockBrandRepository.Verify(r => r.AddAsync(It.IsAny<Brand>()), Times.Once);
        }

        [Test]
        public async Task AddBrandAsync_ShouldSetDefaultImageUrl_WhenInputLogoUrlIsNull()
        {
            var userId = Guid.NewGuid().ToString();

            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            Brand? addedBrand = null;
            mockBrandRepository
                .Setup(r => r.AddAsync(It.IsAny<Brand>()))
                .Callback<Brand>(b => addedBrand = b)
                .Returns(Task.CompletedTask);

            var input = new BrandFormInputViewModel
            {
                Name = "Brand Name",
                LogoUrl = null,
                Description = "Some description"
            };

            var result = await brandService.AddBrandAsync(userId, input);

            Assert.That(result, Is.True);
            Assert.That(addedBrand, Is.Not.Null);
            Assert.That(addedBrand!.LogoUrl, Is.EqualTo(DefaultImageUrl));

        }

        [Test]
        public async Task GetEditableBrandByIdAsync_ShouldReturnNull_WhenBrandIdIsNull()
        {
            var result = await brandService.GetEditableBrandByIdAsync(Guid.NewGuid().ToString(), null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditableBrandByIdAsync_ShouldReturnNull_WhenBrandIdIsLessThanOrEqualToZero()
        {
            var result = await brandService.GetEditableBrandByIdAsync(Guid.NewGuid().ToString(), 0);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditableBrandByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            mockUserRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            var result = await brandService.GetEditableBrandByIdAsync(Guid.NewGuid().ToString(), 1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditableBrandByIdAsync_ShouldReturnNull_WhenBrandDoesNotExist()
        {
            mockUserRepository
        .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
        .ReturnsAsync(new User());

            var mockQueryable = new List<Brand>().AsQueryable().BuildMock();

            mockBrandRepository
                .Setup(x => x.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetEditableBrandByIdAsync(Guid.NewGuid().ToString(), 1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditableBrandByIdAsync_ShouldReturnBrand_WhenUserAndBrandExist()
        {
            var brand = new Brand
            {
                Id = 1,
                Name = "Test Brand",
                LogoUrl = "logo.jpg",
                Description = "Description"
            };

            mockUserRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var mockQueryable = new List<Brand> { brand }.AsQueryable().BuildMock();

            mockBrandRepository
                .Setup(x => x.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetEditableBrandByIdAsync(Guid.NewGuid().ToString(), 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Test Brand"));
            Assert.That(result.LogoUrl, Is.EqualTo("logo.jpg"));
            Assert.That(result.Description, Is.EqualTo("Description"));
        }

        [Test]
        public async Task GetEditableBrandByIdAsync_ShouldReturnDefaultImage_WhenLogoUrlIsNull()
        {
            var brand = new Brand
            {
                Id = 2,
                Name = "No Logo Brand",
                LogoUrl = null,
                Description = "No logo description"
            };

            mockUserRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var mockQueryable = new List<Brand> { brand }.AsQueryable().BuildMock();

            mockBrandRepository
                .Setup(x => x.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetEditableBrandByIdAsync(Guid.NewGuid().ToString(), 2);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.LogoUrl, Is.EqualTo(DefaultImageUrl));
        }

        [Test]
        public async Task EditBrandAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            mockUserRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            var model = new BrandFormInputViewModel { Id = 1, Name = "Test Brand" };

            var result = await brandService.EditBrandAsync(Guid.NewGuid().ToString(), model);

            Assert.That(result, Is.False);
            mockBrandRepository.Verify(x => x.UpdateAsync(It.IsAny<Brand>()), Times.Never);
        }

        [Test]
        public async Task EditBrandAsync_ShouldReturnFalse_WhenBrandDoesNotExist()
        {
            mockUserRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            mockBrandRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Brand?)null);

            var model = new BrandFormInputViewModel { Id = 2, Name = "Non-existent Brand" };

            var result = await brandService.EditBrandAsync(Guid.NewGuid().ToString(), model);

            Assert.That(result, Is.False);
            mockBrandRepository.Verify(x => x.UpdateAsync(It.IsAny<Brand>()), Times.Never);
        }

        [Test]
        public async Task EditBrandAsync_ShouldUpdateBrand_WhenAllValid()
        {
            var user = new User();
            var brand = new Brand { Id = 3, Name = "Old Brand", LogoUrl = "old.jpg", Description = "Old" };

            mockUserRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);

            mockBrandRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(brand);

            mockBrandRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Brand>()))
                .Verifiable();

            var model = new BrandFormInputViewModel
            {
                Id = 3,
                Name = "  New Brand  ",
                LogoUrl = "new.jpg",
                Description = "Updated description"
            };

            var result = await brandService.EditBrandAsync(Guid.NewGuid().ToString(), model);

            Assert.That(result, Is.True);
            Assert.That(brand.Name, Is.EqualTo("New Brand")); // trimmed
            Assert.That(brand.LogoUrl, Is.EqualTo("new.jpg"));
            Assert.That(brand.Description, Is.EqualTo("Updated description"));

            mockBrandRepository.Verify(x => x.UpdateAsync(It.Is<Brand>(b =>
                b.Id == 3 &&
                b.Name == "New Brand" &&
                b.LogoUrl == "new.jpg" &&
                b.Description == "Updated description"
            )), Times.Once);
        }

        [Test]
        public async Task EditBrandAsync_ShouldUseDefaultLogo_WhenLogoUrlIsNull()
        {
            var user = new User();
            var brand = new Brand { Id = 4, Name = "Old", LogoUrl = null, Description = "Old Desc" };

            mockUserRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);

            mockBrandRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(brand);

            mockBrandRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Brand>()))
                .Verifiable();

            var model = new BrandFormInputViewModel
            {
                Id = 4,
                Name = "Updated",
                LogoUrl = null,
                Description = "Desc"
            };

            var result = await brandService.EditBrandAsync(Guid.NewGuid().ToString(), model);

            Assert.That(result, Is.True);
            Assert.That(brand.LogoUrl, Is.EqualTo(DefaultImageUrl));
            mockBrandRepository.Verify(x => x.UpdateAsync(It.IsAny<Brand>()), Times.Once);
        }

        [Test]
        public async Task GetBrandsDropDownDataAsync_ShouldReturnMappedModels_WhenBrandsExist()
        {
            var brands = new List<Brand>
            {
                new Brand { Id = 1, Name = "Brand A" },
                new Brand { Id = 2, Name = "Brand B" }
            };

            var mockQueryable = brands.AsQueryable().BuildMock();

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandsDropDownDataAsync();

            var list = result.ToList();

            Assert.That(list, Is.Not.Null);
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0].Id, Is.EqualTo(1));
            Assert.That(list[0].Name, Is.EqualTo("Brand A"));
            Assert.That(list[1].Id, Is.EqualTo(2));
            Assert.That(list[1].Name, Is.EqualTo("Brand B"));
        }

        [Test]
        public async Task GetBrandsDropDownDataAsync_ShouldReturnEmptyList_WhenNoBrandsExist()
        {
            var emptyBrands = new List<Brand>();

            var mockQueryable = emptyBrands.AsQueryable().BuildMock();

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandsDropDownDataAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetBrandsDropDownDataAsync_ShouldHandleNullName_WhenBrandNameIsNull()
        {
            var brands = new List<Brand>
            {
            new Brand { Id = 1, Name = null! }
            };

            var mockQueryable = brands.AsQueryable().BuildMock();

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandsDropDownDataAsync();

            Assert.That(result, Is.Not.Null);
            var brandResult = result.First();
            Assert.That(brandResult.Id, Is.EqualTo(1));
            Assert.That(brandResult.Name, Is.Null);
        }

        [Test]
        public async Task GetBrandForDeleteByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            var result = await brandService.GetBrandForDeleteByIdAsync(Guid.NewGuid().ToString(), 1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetBrandForDeleteByIdAsync_ShouldReturnNull_WhenBrandIdIsNull()
        {
            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var result = await brandService.GetBrandForDeleteByIdAsync(Guid.NewGuid().ToString(), null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetBrandForDeleteByIdAsync_ShouldReturnNull_WhenBrandDoesNotExist()
        {
            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var mockQueryable = new List<Brand>().AsQueryable().BuildMock();

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandForDeleteByIdAsync(Guid.NewGuid().ToString(), 10);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetBrandForDeleteByIdAsync_ShouldReturnNull_WhenBrandIsDeleted()
        {
            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var brands = new List<Brand>
            {
                 new Brand { Id = 2, Name = "Deleted Brand", IsDeleted = true }
            };

            var mockQueryable = brands.AsQueryable().BuildMock();

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandForDeleteByIdAsync(Guid.NewGuid().ToString(), 2);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetBrandForDeleteByIdAsync_ShouldReturnBrandViewModel_WhenValid()
        {
            var brand = new Brand
            {
                Id = 3,
                Name = "Valid Brand",
                Description = "Valid Description",
                LogoUrl = "logo.png",
                IsDeleted = false
            };

            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var mockQueryable = new List<Brand> { brand }.AsQueryable().BuildMock();

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);

            var result = await brandService.GetBrandForDeleteByIdAsync(Guid.NewGuid().ToString(), 3);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(3));
            Assert.That(result.Name, Is.EqualTo("Valid Brand"));
            Assert.That(result.Description, Is.EqualTo("Valid Description"));
            Assert.That(result.LogoUrl, Is.EqualTo("logo.png"));
        }

        [Test]
        public async Task SoftDeleteBrandAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            mockBrandRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Brand());

            var deleteModel = new DeleteBrandViewModel { Id = 1 };

            var result = await brandService.SoftDeleteBrandAsync(Guid.NewGuid().ToString(), deleteModel);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task SoftDeleteBrandAsync_ShouldReturnFalse_WhenBrandDoesNotExist()
        {
            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            mockBrandRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Brand?)null);

            var deleteModel = new DeleteBrandViewModel { Id = 1 };

            var result = await brandService.SoftDeleteBrandAsync(Guid.NewGuid().ToString(), deleteModel);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task SoftDeleteBrandAsync_ShouldReturnTrue_WhenUserAndBrandExist_AndDeletionSucceeds()
        {
            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var brand = new Brand { Id = 1 };

            mockBrandRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(brand);

            mockBrandRepository
                .Setup(r => r.DeleteAsync(brand))
                .ReturnsAsync(true);

            var deleteModel = new DeleteBrandViewModel { Id = 1 };

            var result = await brandService.SoftDeleteBrandAsync(Guid.NewGuid().ToString(), deleteModel);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task SoftDeleteBrandAsync_ShouldReturnFalse_WhenUserAndBrandExist_AndDeletionFails()
        {
            mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var brand = new Brand { Id = 2 };

            mockBrandRepository
                .Setup(r => r.GetByIdAsync(2))
                .ReturnsAsync(brand);

            mockBrandRepository
                .Setup(r => r.DeleteAsync(brand))
                .ReturnsAsync(false);

            var deleteModel = new DeleteBrandViewModel { Id = 2 };

            var result = await brandService.SoftDeleteBrandAsync(Guid.NewGuid().ToString(), deleteModel);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RestoreByIdAsync_BrandDoesNotExist_ReturnsFalse()
        {
            var data = new List<Brand>
            {
                new Brand { Id = 2, IsDeleted = true }
            };

            var mockData = MockHelper.CreateMockQueryable(data);

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockData);

            var result = await brandService.RestoreByIdAsync(1);

            Assert.That(result, Is.False);
            mockBrandRepository.Verify(r => r.UpdateAsync(It.IsAny<Brand>()), Times.Never);
        }

        [Test]
        public async Task RestoreByIdAsync_BrandIsNotDeleted_ReturnsFalse()
        {
            var data = new List<Brand>
            {
                new Brand { Id = 1, IsDeleted = false }
            };

            var mockData = MockHelper.CreateMockQueryable(data);

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockData);

            var result = await brandService.RestoreByIdAsync(1);

            Assert.That(result, Is.False);
            mockBrandRepository.Verify(r => r.UpdateAsync(It.IsAny<Brand>()), Times.Never);
        }

        [Test]
        public async Task RestoreByIdAsync_DeletedBrandExists_ReturnsTrueAndRestores()
        {
            var brand = new Brand { Id = 1, IsDeleted = true };

            var data = new List<Brand> { brand };

            var mockData = MockHelper.CreateMockQueryable(data);

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockData);

            mockBrandRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Brand>()))
                .ReturnsAsync(true);

            var result = await brandService.RestoreByIdAsync(1);

            Assert.That(result, Is.True);
            Assert.That(brand.IsDeleted, Is.False);
            mockBrandRepository.Verify(r => r.UpdateAsync(It.Is<Brand>(b => b.Id == 1 && !b.IsDeleted)), Times.Once);
        }

        [Test]
        public async Task ExistsByNameAsync_WhenBrandExists_ReturnsTrue()
        {
            string name = "Samsung";
            int skipId = 1;

            mockBrandRepository
                .Setup(r => r.ExistsByNameAsync(name, skipId))
                .ReturnsAsync(true);

            var result = await brandService.ExistsByNameAsync(name, skipId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsByNameAsync_WhenBrandDoesNotExist_ReturnsFalse()
        {
            string name = "NonExistingBrand";
            int skipId = 2;

            mockBrandRepository
                .Setup(r => r.ExistsByNameAsync(name, skipId))
                .ReturnsAsync(false);

            var result = await brandService.ExistsByNameAsync(name, skipId);

            Assert.That(result, Is.False);
        }


        [Test]
        public async Task GetDeletedBrandByNameAsync_WhenDeletedBrandExists_ReturnsBrand()
        {
            string name = "DeletedBrand";
            var deletedBrand = new Brand { Id = 5, Name = name, IsDeleted = true };

            mockBrandRepository
                .Setup(r => r.GetDeletedBrandByNameAsync(name))
                .ReturnsAsync(deletedBrand);

            var result = await brandService.GetDeletedBrandByNameAsync(name);

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Name, Is.EqualTo(name));
            Assert.That(result?.IsDeleted, Is.True);
        }

        [Test]
        public async Task GetDeletedBrandByNameAsync_WhenDeletedBrandDoesNotExist_ReturnsNull()
        {
            string name = "NonDeletedBrand";

            mockBrandRepository
                .Setup(r => r.GetDeletedBrandByNameAsync(name))
                .ReturnsAsync((Brand?)null);

            var result = await brandService.GetDeletedBrandByNameAsync(name);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetPagedAsync_WhenCalled_ReturnsCorrectPagedBrands()
        {
            var brands = new List<Brand>
            {
                new Brand { Id = 1, Name = "Apple", LogoUrl = null, Description = "Tech" },
                new Brand { Id = 2, Name = "Samsung", LogoUrl = "logo.png", Description = "Electronics" },
                new Brand { Id = 3, Name = "Xiaomi", LogoUrl = null, Description = "Affordable" },
            };

            var queryableBrands = brands.AsQueryable().BuildMock();

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableBrands);

            int page = 1;
            int pageSize = 2;

            var result = (await brandService.GetPagedAsync(page, pageSize)).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Apple"));
            Assert.That(result[0].LogoUrl, Is.EqualTo(DefaultImageUrl));
            Assert.That(result[1].Name, Is.EqualTo("Samsung"));
            Assert.That(result[1].LogoUrl, Is.EqualTo("logo.png"));
        }

        [Test]
        public async Task GetPagedAsync_WhenPageIsTooHigh_ReturnsEmptyList()
        {
            var brands = new List<Brand>
            {
                new Brand { Id = 1, Name = "Apple" },
                new Brand { Id = 2, Name = "Samsung" }
            };

            var queryableBrands = brands.AsQueryable().BuildMock();

            mockBrandRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableBrands);

            int page = 5;
            int pageSize = 2;

            var result = (await brandService.GetPagedAsync(page, pageSize)).ToList();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetTotalCountAsync_WhenBrandsExist_ReturnsCorrectCount()
        {
            int expectedCount = 5;

            mockBrandRepository
                .Setup(r => r.CountAsync())
                .ReturnsAsync(expectedCount);

            var result = await brandService.GetTotalCountAsync();

            Assert.That(result, Is.EqualTo(expectedCount));
        }

        [Test]
        public async Task GetTotalCountAsync_WhenNoBrandsExist_ReturnsZero()
        {
            mockBrandRepository
                .Setup(r => r.CountAsync())
                .ReturnsAsync(0);

            var result = await brandService.GetTotalCountAsync();

            Assert.That(result, Is.EqualTo(0));
        }
    }
}
