using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TechStore.Data.Models;
using TechStore.Data.Models.Enums;
using TechStore.Data.Repository.Interfaces;
using TechStore.GCommon;
using TechStore.Web.ViewModels.Product;

namespace TechStore.Services.Core.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<IProductRepository> mockProductRepository = null!;
        private Mock<ICategoryRepository> mockCategoryRepository = null!;
        private ProductService productService = null!;

        private const string TestName = "iPhone";
        private const string TestDescription = "Test description";
        private const string TestImageUrl = "image.png";
        private const decimal TestPrice = 1000m;
        private const int TestQuantity = 10;
        private const int TestCategoryId = 1;
        private const int TestBrandId = 1;

        [SetUp]
        public void SetUp()
        {
            this.mockProductRepository = new Mock<IProductRepository>();
            this.mockCategoryRepository = new Mock<ICategoryRepository>();
            this.productService = new ProductService(mockProductRepository.Object, mockCategoryRepository.Object);
        }

        [Test]
        public void PassAlways()
        {
            // Test that will always pass to show that the SetUp is working
            Assert.Pass();
        }

        [Test]
        public async Task GetProductsByCategoryAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            var categoryId = 1;

            SetupCategory(categoryId, null);

            var result = await productService.GetProductsByCategoryAsync(categoryId, ProductSort.NameAsc);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetProductsByCategoryAsync_ShouldReturnEmptyCategory_WhenNoProducts()
        {
            var categoryId = 1;
            var category = CreateCategory(categoryId, "Phones");

            SetupCategory(categoryId, category);

            var products = new List<Product>();
            var mockQueryable = CreateMockQueryable(products);

            SetupProductsQuery(categoryId, mockQueryable);

            var result = await productService.GetProductsByCategoryAsync(categoryId, ProductSort.NameAsc);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Products, Is.Empty);
            Assert.That(result.CategoryName, Is.EqualTo(category.Name));
        }

        [Test]
        public async Task GetProductsByCategoryAsync_ShouldSortByName_WhenDefaultSort()
        {
            var categoryId = 1;
            var category = CreateCategory(categoryId, "Phones");

            var product1 = CreateProduct(Guid.NewGuid(), TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);
            var product2 = CreateProduct(Guid.NewGuid(), "A Product", TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            var products = new List<Product> { product1, product2 };

            SetupCategory(categoryId, category);
            SetupProductsQuery(categoryId, CreateMockQueryable(products));

            var result = await productService.GetProductsByCategoryAsync(categoryId, ProductSort.NameAsc);

            var resultList = result!.Products.ToList();

            Assert.That(resultList[0].Name, Is.EqualTo("A Product"));
            Assert.That(resultList[1].Name, Is.EqualTo(TestName));
        }

        [Test]
        public async Task GetProductsByCategoryAsync_ShouldSortByPriceAscending()
        {
            var categoryId = 1;
            var category = CreateCategory(categoryId, "Phones");

            var product1 = CreateProduct(Guid.NewGuid(), TestName, TestDescription, 2000, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);
            var product2 = CreateProduct(Guid.NewGuid(), TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            SetupCategory(categoryId, category);
            SetupProductsQuery(categoryId, CreateMockQueryable(new List<Product> { product1, product2 }));

            var result = await productService.GetProductsByCategoryAsync(categoryId, ProductSort.PriceAsc);

            var list = result!.Products.ToList();

            Assert.That(list[0].Price, Is.EqualTo(product2.Price));
            Assert.That(list[1].Price, Is.EqualTo(product1.Price));
        }

        [Test]
        public async Task GetProductsByCategoryAsync_ShouldSortByPriceDescending()
        {
            var categoryId = 1;
            var category = CreateCategory(categoryId, "Phones");

            var product1 = CreateProduct(Guid.NewGuid(), TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);
            var product2 = CreateProduct(Guid.NewGuid(), TestName, TestDescription, 2000, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            SetupCategory(categoryId, category);
            SetupProductsQuery(categoryId, CreateMockQueryable(new List<Product> { product1, product2 }));

            var result = await productService.GetProductsByCategoryAsync(categoryId, ProductSort.PriceDesc);

            var list = result!.Products.ToList();

            Assert.That(list[0].Price, Is.EqualTo(product2.Price));
            Assert.That(list[1].Price, Is.EqualTo(product1.Price));
        }

        [Test]
        public async Task GetProductsByCategoryAsync_ShouldMapProductsCorrectlyAndUseDefaultImage_WhenImageNotSet()
        {
            var categoryId = 1;
            var category = CreateCategory(categoryId, "Phones");

            var product = CreateProduct(Guid.NewGuid(), TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId);

            SetupCategory(categoryId, category);
            SetupProductsQuery(categoryId, CreateMockQueryable(new List<Product> { product }));

            var result = await productService.GetProductsByCategoryAsync(categoryId, ProductSort.NameAsc);

            var item = result!.Products.First();

            Assert.That(item.Id, Is.EqualTo(product.Id));
            Assert.That(item.Name, Is.EqualTo(product.Name));
            Assert.That(item.Price, Is.EqualTo(product.Price));
            Assert.That(item.QuantityInStock, Is.EqualTo(product.QuantityInStock));
            Assert.That(item.ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));
        }

        [Test]
        public async Task GetProductDetailsViewModelAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();

            var products = new List<Product>();
            SetupProducts(CreateMockQueryable(products));

            var result = await productService.GetProductDetailsViewModelAsync(productId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetProductDetailsViewModelAsync_ShouldReturnCorrectViewModel_WhenProductExists()
        {
            var productId = Guid.NewGuid();

            var brand = new Brand
            {
                Id = 1,
                Name = "Apple"
            };

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, brand.Id, null, brand);

            var products = new List<Product> { product };

            SetupProducts(CreateMockQueryable(products));

            var result = await productService.GetProductDetailsViewModelAsync(productId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(product.Id));
            Assert.That(result.Name, Is.EqualTo(product.Name));
            Assert.That(result.Description, Is.EqualTo(product.Description));
            Assert.That(result.Price, Is.EqualTo(product.Price));
            Assert.That(result.QuantityInStock, Is.EqualTo(product.QuantityInStock));
            Assert.That(result.CategoryId, Is.EqualTo(product.CategoryId));
            Assert.That(result.BrandId, Is.EqualTo(product.BrandId));
            Assert.That(result.ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl)); //Correct use of DefaultImageUrl
        }

        [Test]
        public async Task AddProductAsync_ShouldAddProduct_WithCorrectData()
        {
            var inputModel = new ProductFormInputModel
            {
                Name = " iPhone ",
                Description = TestDescription,
                ImageUrl = TestImageUrl,
                Price = TestPrice,
                QuantityInStock = TestQuantity,
                CategoryId = TestCategoryId,
                BrandId = TestBrandId
            };

            Product? addedProduct = null;

            mockProductRepository
                .Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Callback<Product>(p => addedProduct = p);

            await productService.AddProductAsync(inputModel);

            Assert.That(addedProduct, Is.Not.Null);

            Assert.That(addedProduct!.Name, Is.EqualTo("iPhone")); // Trimmed
            Assert.That(addedProduct.Description, Is.EqualTo(inputModel.Description));
            Assert.That(addedProduct.ImageUrl, Is.EqualTo(inputModel.ImageUrl));
            Assert.That(addedProduct.Price, Is.EqualTo(inputModel.Price));
            Assert.That(addedProduct.QuantityInStock, Is.EqualTo(inputModel.QuantityInStock));
            Assert.That(addedProduct.CategoryId, Is.EqualTo(inputModel.CategoryId));
            Assert.That(addedProduct.BrandId, Is.EqualTo(inputModel.BrandId));

            mockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task AddProductAsync_ShouldUseDefaultImage_WhenImageUrlIsNull()
        {
            var inputModel = new ProductFormInputModel
            {
                Name = TestName,
                Description = TestDescription,
                ImageUrl = null,
                Price = TestPrice,
                QuantityInStock = TestQuantity,
                CategoryId = TestCategoryId,
                BrandId = TestBrandId
            };

            Product? addedProduct = null;

            mockProductRepository
                .Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Callback<Product>(p => addedProduct = p);

            await productService.AddProductAsync(inputModel);

            Assert.That(addedProduct, Is.Not.Null);
            Assert.That(addedProduct!.ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));

            mockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetEditableProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();

            SetupProductById(productId, null);

            var result = await productService.GetEditableProductByIdAsync(productId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditableProductByIdAsync_ShouldReturnCorrectModel_WhenProductExists()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            SetupProductById(productId, product);

            var result = await productService.GetEditableProductByIdAsync(productId);

            Assert.That(result, Is.Not.Null);

            Assert.That(result!.Id, Is.EqualTo(product.Id));
            Assert.That(result.Name, Is.EqualTo(product.Name));
            Assert.That(result.Description, Is.EqualTo(product.Description));
            Assert.That(result.Price, Is.EqualTo(product.Price));
            Assert.That(result.QuantityInStock, Is.EqualTo(product.QuantityInStock));
            Assert.That(result.CategoryId, Is.EqualTo(product.CategoryId));
            Assert.That(result.BrandId, Is.EqualTo(product.BrandId));
            Assert.That(result.ImageUrl, Is.EqualTo(product.ImageUrl));
        }

        [Test]
        public async Task GetEditableProductByIdAsync_ShouldKeepNullImageUrl_WhenImageIsNull()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, null);

            SetupProductById(productId, product);

            var result = await productService.GetEditableProductByIdAsync(productId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ImageUrl, Is.Null);
        }

        [Test]
        public async Task EditProductAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();

            var inputModel = CreateProductInput();

            SetupProductById(productId, null);

            var result = await productService.EditProductAsync(inputModel);

            Assert.That(result, Is.False);

            mockProductRepository.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
            mockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task EditProductAsync_ShouldUpdateProductAndReturnTrue_WhenProductExists()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            var inputModel = new ProductFormInputModel
            {
                Id = productId,
                Name = " NewName ",
                Description = "NewDesc",
                ImageUrl = "new.png",
                Price = 1500,
                QuantityInStock = 20,
                CategoryId = 2,
                BrandId = 2
            };

            SetupProductById(productId, product);

            var result = await productService.EditProductAsync(inputModel);

            Assert.That(result, Is.True);

            Assert.That(product.Name, Is.EqualTo("NewName")); // Trimmed
            Assert.That(product.Description, Is.EqualTo(inputModel.Description));
            Assert.That(product.ImageUrl, Is.EqualTo(inputModel.ImageUrl));
            Assert.That(product.Price, Is.EqualTo(inputModel.Price));
            Assert.That(product.QuantityInStock, Is.EqualTo(inputModel.QuantityInStock));
            Assert.That(product.CategoryId, Is.EqualTo(inputModel.CategoryId));
            Assert.That(product.BrandId, Is.EqualTo(inputModel.BrandId));

            mockProductRepository.Verify(r => r.Update(product), Times.Once);
            mockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task EditProductAsync_ShouldUseDefaultImage_WhenImageUrlIsNull()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            var inputModel = new ProductFormInputModel
            {
                Id = productId,
                Name = TestName,
                Description = TestDescription,
                ImageUrl = null,
                Price = TestPrice,
                QuantityInStock = TestQuantity,
                CategoryId = TestCategoryId,
                BrandId = TestBrandId
            };

            SetupProductById(productId, product);

            var result = await productService.EditProductAsync(inputModel);

            Assert.That(result, Is.True);
            Assert.That(product.ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));

            mockProductRepository.Verify(r => r.Update(product), Times.Once);
            mockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetProductForDeleteAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();

            SetupProducts(CreateMockQueryable(new List<Product>()));

            var result = await productService.GetProductForDeleteAsync(productId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetProductForDeleteAsync_ShouldReturnNull_WhenProductIsDeleted()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product.IsDeleted = true;

            SetupProducts(CreateMockQueryable(new List<Product> { product }));

            var result = await productService.GetProductForDeleteAsync(productId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetProductForDeleteAsync_ShouldReturnNull_WhenProductHasPendingOrders()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product.OrdersProducts = new List<OrderProduct>
            {
                new OrderProduct
                {
                    Order = new Order
                    {
                        Status = 0
                    }
                }
            };

            SetupProducts(CreateMockQueryable(new List<Product> { product }));

            var result = await productService.GetProductForDeleteAsync(productId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetProductForDeleteAsync_ShouldMapCorrectly()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId);

            product.OrdersProducts = new List<OrderProduct>();

            SetupProducts(CreateMockQueryable(new List<Product> { product }));

            var result = await productService.GetProductForDeleteAsync(productId);

            Assert.That(result, Is.Not.Null);

            Assert.That(result!.Id, Is.EqualTo(product.Id));
            Assert.That(result.Name, Is.EqualTo(product.Name));
            Assert.That(result.Description, Is.EqualTo(product.Description));
            Assert.That(result.CategoryId, Is.EqualTo(product.CategoryId));
            Assert.That(result.ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl)); //Correct use of DefaultImageUrl
        }

        [Test]
        public async Task SoftDeleteProductAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();

            SetupProducts(CreateMockQueryable(new List<Product>()));

            var model = new DeleteProductViewModel { Id = productId };

            var result = await productService.SoftDeleteProductAsync(model);

            Assert.That(result, Is.False);

            mockProductRepository.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
            mockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task SoftDeleteProductAsync_ShouldReturnFalse_WhenProductIsAlreadyDeleted()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product.IsDeleted = true;

            SetupProducts(CreateMockQueryable(new List<Product> { product }));

            var model = new DeleteProductViewModel { Id = productId };

            var result = await productService.SoftDeleteProductAsync(model);

            Assert.That(result, Is.False);

            mockProductRepository.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
        }

        [Test]
        public async Task SoftDeleteProductAsync_ShouldReturnFalse_WhenProductHasPendingOrders()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product.OrdersProducts = new List<OrderProduct>
            {
                CreateOrderProduct(0)
            };

            SetupProducts(CreateMockQueryable(new List<Product> { product }));

            var model = new DeleteProductViewModel { Id = productId };

            var result = await productService.SoftDeleteProductAsync(model);

            Assert.That(result, Is.False);

            mockProductRepository.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
            mockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task SoftDeleteProductAsync_ShouldDeleteProductAndReturnTrue_WhenValid()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product.OrdersProducts = new List<OrderProduct>
            {
                CreateOrderProduct(3)
            };

            SetupProducts(CreateMockQueryable(new List<Product> { product }));

            var model = new DeleteProductViewModel { Id = productId };

            var result = await productService.SoftDeleteProductAsync(model);

            Assert.That(result, Is.True);

            mockProductRepository.Verify(r => r.Delete(product), Times.Once);
            mockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task SearchByKeywordAsync_ShouldReturnEmptyCollection_WhenNoProducts()
        {
            var keyword = "test";

            mockProductRepository
                .Setup(r => r.SearchByKeywordAsync(keyword))
                .ReturnsAsync(new List<Product>());

            var result = await productService.SearchByKeywordAsync(keyword);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task SearchByKeywordAsync_ShouldMapProductsCorrectly()
        {
            var keyword = "phone";

            var product = CreateProduct(Guid.NewGuid(), TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            mockProductRepository
                .Setup(r => r.SearchByKeywordAsync(keyword))
                .ReturnsAsync(new List<Product> { product });

            var result = (await productService.SearchByKeywordAsync(keyword)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));

            var item = result[0];

            Assert.That(item.Id, Is.EqualTo(product.Id));
            Assert.That(item.Name, Is.EqualTo(product.Name));
            Assert.That(item.ImageUrl, Is.EqualTo(product.ImageUrl));
            Assert.That(item.Price, Is.EqualTo(product.Price));
            Assert.That(item.QuantityInStock, Is.EqualTo(product.QuantityInStock));
        }

        [Test]
        public async Task SearchByKeywordAsync_ShouldOrderByName()
        {
            var keyword = "test";

            var product1 = CreateProduct(Guid.NewGuid(), "Z Product", TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);
            var product2 = CreateProduct(Guid.NewGuid(), "A Product", TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            mockProductRepository
                .Setup(r => r.SearchByKeywordAsync(keyword))
                .ReturnsAsync(new List<Product> { product1, product2 });

            var result = (await productService.SearchByKeywordAsync(keyword)).ToList();

            Assert.That(result[0].Name, Is.EqualTo(product2.Name));
            Assert.That(result[1].Name, Is.EqualTo(product1.Name));
        }

        [Test]
        public async Task SearchByKeywordAsync_ShouldUseDefaultImage_WhenImageUrlIsNull()
        {
            var keyword = "test";

            var product = CreateProduct(Guid.NewGuid(), TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId);

            mockProductRepository
                .Setup(r => r.SearchByKeywordAsync(keyword))
                .ReturnsAsync(new List<Product> { product });

            var result = (await productService.SearchByKeywordAsync(keyword)).ToList();

            Assert.That(result[0].ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));
        }

        [Test]
        public async Task ExistsByNameAsync_ShouldReturnTrue_WhenProductExists()
        {
            var name = TestName;
            string? skipId = null;

            mockProductRepository
                .Setup(r => r.ExistsByNameAsync(name, skipId))
                .ReturnsAsync(true);

            var result = await productService.ExistsByNameAsync(name, skipId);

            Assert.That(result, Is.True);

            mockProductRepository.Verify(r => r.ExistsByNameAsync(name, skipId), Times.Once);
        }

        [Test]
        public async Task ExistsByNameAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            var name = TestName;
            string? skipId = null;

            mockProductRepository
                .Setup(r => r.ExistsByNameAsync(name, skipId))
                .ReturnsAsync(false);

            var result = await productService.ExistsByNameAsync(name, skipId);

            Assert.That(result, Is.False);

            mockProductRepository.Verify(r => r.ExistsByNameAsync(name, skipId), Times.Once);
        }

        [Test]
        public async Task GetDeletedProductByNameAsync_ShouldReturnProduct_WhenDeletedProductExists()
        {
            var name = "iPhone";

            var product = CreateProduct(Guid.NewGuid(), name, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product.IsDeleted = true;

            mockProductRepository
                .Setup(r => r.GetDeletedProductByNameAsync(name))
                .ReturnsAsync(product);

            var result = await productService.GetDeletedProductByNameAsync(name);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo(name));
            Assert.That(result.IsDeleted, Is.True);

            mockProductRepository.Verify(r => r.GetDeletedProductByNameAsync(name), Times.Once);
        }

        [Test]
        public async Task GetDeletedProductByNameAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            var name = "Samsung";

            mockProductRepository
                .Setup(r => r.GetDeletedProductByNameAsync(name))
                .ReturnsAsync((Product?)null);

            var result = await productService.GetDeletedProductByNameAsync(name);

            Assert.That(result, Is.Null);

            mockProductRepository.Verify(r => r.GetDeletedProductByNameAsync(name), Times.Once);
        }

        [Test]
        public async Task GetProductForRestoreByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();

            SetupProducts(CreateMockQueryable(new List<Product>()));

            var result = await productService.GetProductForRestoreByIdAsync(productId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetProductForRestoreByIdAsync_ShouldReturnCorrectModel_WhenProductExists()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            var products = new List<Product> { product };

            SetupProducts(CreateMockQueryable(products));

            var result = await productService.GetProductForRestoreByIdAsync(productId);

            Assert.That(result, Is.Not.Null);

            Assert.That(result!.Id, Is.EqualTo(product.Id));
            Assert.That(result.Name, Is.EqualTo(product.Name));
            Assert.That(result.Description, Is.EqualTo(product.Description));
            Assert.That(result.Price, Is.EqualTo(product.Price));
            Assert.That(result.QuantityInStock, Is.EqualTo(product.QuantityInStock));
            Assert.That(result.CategoryId, Is.EqualTo(product.CategoryId));
            Assert.That(result.BrandId, Is.EqualTo(product.BrandId));
            Assert.That(result.ImageUrl, Is.EqualTo(product.ImageUrl));
        }

        [Test]
        public async Task GetProductForRestoreByIdAsync_ShouldKeepNullImageUrl_WhenImageIsNull()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId);

            SetupProducts(CreateMockQueryable(new List<Product> { product }));

            var result = await productService.GetProductForRestoreByIdAsync(productId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ImageUrl, Is.Null);
        }

        [Test]
        public async Task RestoreByIdAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            var productId = Guid.NewGuid();

            SetupProducts(CreateMockQueryable(new List<Product>()));

            var result = await productService.RestoreByIdAsync(productId);

            Assert.That(result, Is.False);

            mockProductRepository.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
            mockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task RestoreByIdAsync_ShouldReturnFalse_WhenProductIsNotDeleted()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product.IsDeleted = false;

            SetupProducts(CreateMockQueryable(new List<Product> { product }));

            var result = await productService.RestoreByIdAsync(productId);

            Assert.That(result, Is.False);

            mockProductRepository.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
        }

        [Test]
        public async Task RestoreByIdAsync_ShouldRestoreProductAndReturnTrue_WhenProductIsDeleted()
        {
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId, TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product.IsDeleted = true;

            SetupProducts(CreateMockQueryable(new List<Product> { product }));

            var result = await productService.RestoreByIdAsync(productId);

            Assert.That(result, Is.True);
            Assert.That(product.IsDeleted, Is.False);

            mockProductRepository.Verify(r => r.Update(product), Times.Once);
            mockProductRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnEmpty_WhenNoProducts()
        {
            SetupProductCount(0);

            var result = await productService.GetPagedAsync(1, 10);

            Assert.That(result.Products, Is.Empty);
            Assert.That(result.CurrentPage, Is.EqualTo(1));
            Assert.That(result.TotalPages, Is.EqualTo(0));
        }

        [Test]
        public async Task GetPagedAsync_ShouldNormalizePaging_WhenInvalidInput()
        {
            SetupProductCount(0);

            var result = await productService.GetPagedAsync(0, 0);

            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetPagedAsync_ShouldClampPage_WhenPageExceedsTotalPages()
        {
            var product = CreateProduct(Guid.NewGuid(), TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product.Category = CreateCategory(TestCategoryId, "Test Category");
            product.Brand = new Brand { Id = TestBrandId, Name = "Test Brand" };

            SetupProducts(CreateMockQueryable(new List<Product> { product }));
            SetupProductCount(1);

            var result = await productService.GetPagedAsync(10, 5);

            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnCorrectNumberOfProducts()
        {
            var product1 = CreateProduct(Guid.NewGuid(), "Product1", TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);
            var product2 = CreateProduct(Guid.NewGuid(), "Product2", TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product1.Category = CreateCategory(TestCategoryId, "Test Category");
            product2.Category = CreateCategory(TestCategoryId, "Test Category");
            product1.Brand = new Brand { Id = TestBrandId, Name = "Test Brand" };
            product2.Brand = new Brand { Id = TestBrandId, Name = "Test Brand" };

            var products = new List<Product> { product1, product2};

            SetupProducts(CreateMockQueryable(products));
            SetupProductCount(products.Count);

            var result = await productService.GetPagedAsync(1, 5);

            Assert.That(result.Products.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetPagedAsync_ShouldOrderProductsByName()
        {
            var product1 = CreateProduct(Guid.NewGuid(), "Z Product", TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);
            var product2 = CreateProduct(Guid.NewGuid(), "A Product", TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product1.Category = CreateCategory(TestCategoryId, "Test Category");
            product2.Category = CreateCategory(TestCategoryId, "Test Category");
            product1.Brand = new Brand { Id = TestBrandId, Name = "Test Brand" };
            product2.Brand = new Brand { Id = TestBrandId, Name = "Test Brand" };
            
            SetupProducts(CreateMockQueryable(new List<Product> { product1, product2 }));
            SetupProductCount(2);

            var result = (await productService.GetPagedAsync(1, 10)).Products.ToList();

            Assert.That(result[0].Name, Is.EqualTo(product2.Name));
            Assert.That(result[1].Name, Is.EqualTo(product1.Name));
        }

        [Test]
        public async Task GetPagedAsync_ShouldMapProductsCorrectly()
        {
            var product = CreateProduct(Guid.NewGuid(), TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId, TestImageUrl);

            product.Category = new Category { Name = "Phones" };
            product.Brand = new Brand { Name = "Apple" };

            SetupProducts(CreateMockQueryable(new List<Product> { product }));
            SetupProductCount(1);

            var result = (await productService.GetPagedAsync(1, 10)).Products.ToList();

            var item = result[0];

            Assert.That(item.Name, Is.EqualTo(product.Name));
            Assert.That(item.Description, Is.EqualTo(product.Description));
            Assert.That(item.Price, Is.EqualTo(product.Price));
            Assert.That(item.QuantityInStock, Is.EqualTo(product.QuantityInStock));
            Assert.That(item.CategoryName, Is.EqualTo("Phones"));
            Assert.That(item.BrandName, Is.EqualTo("Apple"));
        }

        [Test]
        public async Task GetPagedAsync_ShouldUseDefaultImage_WhenImageUrlIsNull()
        {
            var product = CreateProduct(Guid.NewGuid(), TestName, TestDescription, TestPrice, TestQuantity, TestCategoryId, TestBrandId);

            product.Category = new Category { Name = "Phones" };
            product.Brand = new Brand { Name = "Apple" };

            SetupProducts(CreateMockQueryable(new List<Product> { product }));
            SetupProductCount(1);

            var result = (await productService.GetPagedAsync(1, 10)).Products.First();

            Assert.That(result.ImageUrl, Is.EqualTo(ApplicationConstants.DefaultImageUrl));
        }

        private void SetupCategory(int categoryId, Category? category)
        {
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(category);
        }

        private void SetupProductCount(int count)
        {
            mockProductRepository
                .Setup(r => r.CountAsync())
                .ReturnsAsync(count);
        }

        private void SetupProducts(IQueryable<Product> products)
        {
            mockProductRepository
                .Setup(r => r.GetAllAttached())
                .Returns(products);
        }

        private void SetupProductById(Guid id, Product? product)
        {
            mockProductRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(product);
        }

        private void SetupProductsQuery(int categoryId, IQueryable<Product> products)
        {
            mockProductRepository
                .Setup(r => r.GetByCategoryQuery(categoryId))
                .Returns(products);
        }

        private Category CreateCategory(int id, string name)
        {
            return new Category
            {
                Id = id,
                Name = name
            };
        }

        private Product CreateProduct(Guid id, string name, 
            string description, decimal price, 
            int quantity, int categoryId, 
            int brandId, string? imageUrl = null, Brand? brand = null)
        {
            return new Product
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price,
                QuantityInStock = quantity,
                CategoryId = categoryId,
                BrandId = brandId,
                ImageUrl = imageUrl,
                Brand = brand
            };
        }

        private ProductFormInputModel CreateProductInput(string? imageUrl = TestImageUrl)
        {
            return new ProductFormInputModel
            {
                Name = TestName,
                Description = TestDescription,
                ImageUrl = imageUrl,
                Price = TestPrice,
                QuantityInStock = TestQuantity,
                CategoryId = TestCategoryId,
                BrandId = TestBrandId
            };
        }

        private OrderProduct CreateOrderProduct(int status)
        {
            return new OrderProduct
            {
                Order = new Order
                {
                    Status = (Status)status
                }
            };
        }

        private IQueryable<Product> CreateMockQueryable(List<Product> data)
        {
            return data.AsQueryable().BuildMockDbSet().Object;
        }
    }
}