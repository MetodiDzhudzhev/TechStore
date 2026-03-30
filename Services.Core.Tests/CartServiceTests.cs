using Moq;
using NUnit.Framework;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;

namespace TechStore.Services.Core.Tests
{
    [TestFixture]
    public class CartServiceTests
    {
        private Mock<ICartRepository> mockCartRepository = null!;
        private Mock<IProductRepository> mockProductRepository = null!;
        private CartService cartService = null!;

        [SetUp]
        public void SetUp()
        {
            this.mockCartRepository = new Mock<ICartRepository>();
            this.mockProductRepository = new Mock<IProductRepository>();
            this.cartService = new CartService(mockCartRepository.Object, mockProductRepository.Object);
        }

        [Test]
        public void PassAlways()
        {
            // Test that will always pass to show that the SetUp is working
            Assert.Pass();
        }

        [Test]
        public async Task GetCartAsync_ShouldReturnNull_WhenCartDoesNotExist()
        {
            var cartId = Guid.NewGuid();

            SetupCart(null, cartId);

            var result = await cartService.GetCartAsync(cartId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCartAsync_ShouldReturnCartViewModelWithCorrectProducts_WhenCartExistsAndItIsNotEmpty()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId);
            var cartProduct = CreateCartProduct(cartId, productId, 2, product);
            var cart = CreateCart(cartId, cartProduct);

            SetupCart(cart, cartId);

            var result = await cartService.GetCartAsync(cartId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Products.Count(), Is.EqualTo(1));

            var resultProduct = result.Products.First();

            Assert.That(resultProduct.ProductId, Is.EqualTo(productId));
            Assert.That(resultProduct.Name, Is.EqualTo(product.Name));
            Assert.That(resultProduct.ImageUrl, Is.EqualTo(product.ImageUrl));
            Assert.That(resultProduct.Price, Is.EqualTo(product.Price));
            Assert.That(resultProduct.Quantity, Is.EqualTo(cartProduct.Quantity));
        }

        [Test]
        public async Task GetCartAsync_ShouldReturnEmptyProducts_WhenCartHasNoProducts()
        {
            var cartId = Guid.NewGuid();

            var cart = CreateCart(cartId);
            SetupCart(cart, cartId);

            var result = await cartService.GetCartAsync(cartId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Products, Is.Empty);
        }

        [Test]
        public async Task AddProductAsync_ShouldReturnFalse_WhenQuantityIsLessThanOne()
        {
            var result = await cartService.AddProductAsync(Guid.NewGuid(), Guid.NewGuid(), 0);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddProductAsync_ShouldReturnFalse_WhenCartDoesNotExist()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            SetupCart(null, cartId);

            var result = await cartService.AddProductAsync(cartId, productId, 1);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddProductAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var cart = CreateCart(cartId);
            SetupCart(cart, cartId);
            SetupProduct(null, productId);

            var result = await cartService.AddProductAsync(cartId, productId, 1);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddProductAsync_ShouldReturnFalse_WhenQuantityExceedsStock()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId);
            var cart = CreateCart(cartId);
            SetupCart(cart, cartId);
            SetupProduct(product, productId);

            var result = await cartService.AddProductAsync(cartId, productId, 11);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddProductAsync_ShouldAddNewProduct_WhenNotInCart()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId);
            var cart = CreateCart(cartId);
            SetupCart(cart, cartId);
            SetupProduct(product, productId);

            var result = await cartService.AddProductAsync(cartId, productId, 2);

            Assert.That(result, Is.True);
            Assert.That(cart.Products.Count, Is.EqualTo(1));
            Assert.That(cart.Products.First().Quantity, Is.EqualTo(2));

            mockCartRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task AddProductAsync_ShouldIncreaseQuantity_WhenProductAlreadyInCart()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId);
            var cartProduct = CreateCartProduct(cartId, productId, 2, product);
            var cart = CreateCart(cartId, cartProduct);
            SetupCart(cart, cartId);
            SetupProduct(product, productId);

            var result = await cartService.AddProductAsync(cartId, productId, 3);

            Assert.That(result, Is.True);
            Assert.That(cartProduct.Quantity, Is.EqualTo(5));

            mockCartRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task IncreaseProductQuantityAsync_ShouldIncreaseQuantityByOne()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId);
            var cartProduct = CreateCartProduct(cartId, productId, 2, product);
            var cart = CreateCart(cartId, cartProduct);
            SetupCart(cart, cartId);
            SetupProduct(product, productId);

            var result = await cartService.IncreaseProductQuantityAsync(cartId, productId);

            Assert.That(result, Is.True);
            Assert.That(cartProduct.Quantity, Is.EqualTo(3));

            mockCartRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task IncreaseProductQuantityAsync_ShouldReturnFalse_WhenExceedsStock()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId);
            var cartProduct = CreateCartProduct(cartId, productId, 10, product);
            var cart = CreateCart(cartId, cartProduct);
            SetupCart(cart, cartId);
            SetupProduct(product, productId);

            var result = await cartService.IncreaseProductQuantityAsync(cartId, productId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RemoveProductAsync_ShouldReturnFalse_WhenCartDoesNotExist()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            SetupCart(null, cartId);

            var result = await cartService.RemoveProductAsync(cartId, productId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RemoveProductAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var cart = CreateCart(cartId);
            SetupCart(cart, cartId);
            SetupProduct(null, productId);

            var result = await cartService.RemoveProductAsync(cartId, productId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RemoveProductAsync_ShouldReturnFalse_WhenProductNotInCart()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId);
            var cart = CreateCart(cartId);
            SetupCart(cart, cartId);
            SetupProduct(product, productId);

            var result = await cartService.RemoveProductAsync(cartId, productId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RemoveProductAsync_ShouldRemoveProductAndReturnTrue_WhenProductExistsInCart()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId);
            var cartProduct = CreateCartProduct(cartId, productId, 2, product);
            var cart = CreateCart(cartId, cartProduct);
            SetupCart(cart, cartId);
            SetupProduct(product, productId);

            var result = await cartService.RemoveProductAsync(cartId, productId);

            Assert.That(result, Is.True);
            Assert.That(cart.Products, Is.Empty);

            mockCartRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DecreaseProductAsync_ShouldReturnFalse_WhenCartDoesNotExist()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            SetupCart(null, cartId);

            var result = await cartService.DecreaseProductAsync(cartId, productId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DecreaseProductAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var cart = CreateCart(cartId);
            SetupCart(cart, cartId);
            SetupProduct(null, productId);

            var result = await cartService.DecreaseProductAsync(cartId, productId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DecreaseProductAsync_ShouldReturnFalse_WhenProductNotInCart()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId);
            var cart = CreateCart(cartId);
            SetupCart(cart, cartId);
            SetupProduct(product, productId);

            var result = await cartService.DecreaseProductAsync(cartId, productId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DecreaseProductAsync_ShouldDecreaseQuantity_WhenQuantityGreaterThanOne()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId);
            var cartProduct = CreateCartProduct(cartId, productId, 2, product);
            var cart = CreateCart(cartId, cartProduct);
            SetupCart(cart, cartId);
            SetupProduct(product, productId);

            var result = await cartService.DecreaseProductAsync(cartId, productId);

            Assert.That(result, Is.True);
            Assert.That(cartProduct.Quantity, Is.EqualTo(1));

            mockCartRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DecreaseProductAsync_ShouldNotDecreaseBelowOne_WhenQuantityIsOne()
        {
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = CreateProduct(productId);
            var cartProduct = CreateCartProduct(cartId, productId, 1, product);
            var cart = CreateCart(cartId, cartProduct);
            SetupCart(cart, cartId);
            SetupProduct(product, productId);

            var result = await cartService.DecreaseProductAsync(cartId, productId);

            Assert.That(result, Is.True);
            Assert.That(cartProduct.Quantity, Is.EqualTo(1));

            mockCartRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ClearCartAsync_ShouldReturnFalse_WhenCartDoesNotExist()
        {
            var cartId = Guid.NewGuid();

            SetupCart(null, cartId);

            var result = await cartService.ClearCartAsync(cartId);

            Assert.That(result, Is.False);

            mockCartRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ClearCartAsync_ShouldRemoveAllProductsAndReturnTrue_WhenCartExists()
        {
            var cartId = Guid.NewGuid();
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            var cartProduct1 = CreateCartProduct(cartId, productId1, 2);
            var cartProduct2 = CreateCartProduct(cartId, productId2, 2);
            var cart = CreateCart(cartId, cartProduct1, cartProduct2);
            SetupCart(cart, cartId);

            var result = await cartService.ClearCartAsync(cartId);

            Assert.That(result, Is.True);
            Assert.That(cart.Products, Is.Empty);

            mockCartRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetCartItemsCountAsync_ShouldReturnCorrectCount()
        {
            var cartId = Guid.NewGuid();

            mockCartRepository
                .Setup(r => r.GetCartItemsCountAsync(cartId))
                .ReturnsAsync(5);

            var result = await cartService.GetCartItemsCountAsync(cartId);

            Assert.That(result, Is.EqualTo(5));

            mockCartRepository.Verify(r => r.GetCartItemsCountAsync(cartId), Times.Once);
        }

        [Test]
        public async Task GetCartItemsCountAsync_ShouldReturnZero_WhenNoItems()
        {
            var cartId = Guid.NewGuid();

            mockCartRepository
                .Setup(r => r.GetCartItemsCountAsync(cartId))
                .ReturnsAsync(0);

            var result = await cartService.GetCartItemsCountAsync(cartId);

            Assert.That(result, Is.EqualTo(0));

            mockCartRepository.Verify(r => r.GetCartItemsCountAsync(cartId), Times.Once);
        }

        private Cart CreateCart(Guid cartId, params CartProduct[] products)
        {
            return new Cart
            {
                Id = cartId,
                Products = products.ToList()
            };
        }

        private CartProduct CreateCartProduct(Guid cartId, Guid productId, int quantity, Product? product = null)
        {
            return new CartProduct
            {
                CartId = cartId,
                ProductId = productId,
                Quantity = quantity,
                Product = product
            };
        }

        private Product CreateProduct(Guid productId, int stock = 10)
        {
            return new Product
            {
                Id = productId,
                QuantityInStock = stock,
                Name = "Test Product",
                ImageUrl = "image.png",
                Price = 100
            };
        }

        private void SetupCart(Cart? cart, Guid cartId)
        {
            mockCartRepository
                .Setup(r => r.GetCartWithProductsAsync(cartId))
                .ReturnsAsync(cart);
        }

        private void SetupProduct(Product? product, Guid productId)
        {
            mockProductRepository
                .Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(product);
        }
    }
}