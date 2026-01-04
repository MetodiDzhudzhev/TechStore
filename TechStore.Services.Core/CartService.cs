using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Cart;
using TechStore.Web.ViewModels.CartProduct;
namespace TechStore.Services.Core
{
    public class CartService : ICartService
    {
        private readonly ICartRepository cartRepository;
        private readonly IProductRepository productRepository;

        public CartService(ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            this.cartRepository = cartRepository;
            this.productRepository = productRepository;
        }

        public async Task<CartViewModel?> GetCartAsync(string? id)
        {
            bool isIdValid = Guid.TryParse(id, out Guid cartId);

            if (!isIdValid)
            {
                return null;
            }

            Cart? cart = await cartRepository.GetCartWithProductsAsync(cartId);

            if (cart == null)
            {
                return null;
            }

            return new CartViewModel
            {
                Products = cart.Products.Select(cp => new CartProductDetailsViewModel
                {
                    ProductId = cp.ProductId,
                    Name = cp.Product.Name,
                    ImageUrl = cp.Product.ImageUrl,
                    Price = cp.Product.Price,
                    Quantity = cp.Quantity
                })
            };
        }

        public async Task<bool> AddProductAsync(string cartId, string? productId, int quantity)
        {
            bool isCartIdValid = Guid.TryParse(cartId, out Guid currentCartId);

            if (!isCartIdValid)
            {
                return false;
            }

            Cart? cart = await this.cartRepository.GetCartWithProductsAsync(currentCartId);

            if (cart == null || quantity <= 0)
            {
                return false;
            }

            bool isProductIdValid = Guid.TryParse(productId, out Guid currentProductId);

            if (!isProductIdValid)
            {
                return false;
            }

            Product? product = await productRepository.GetByIdAsync(currentProductId);

            if (product == null)
            {
                return false;
            }

            CartProduct? cartProduct = cart.Products.SingleOrDefault(cp => cp.ProductId == currentProductId);

            if (cartProduct == null)
            {
                cart.Products.Add(new CartProduct
                {
                    CartId = cart.Id,
                    ProductId = currentProductId,
                    Quantity = quantity
                });
            }
            else
            {
                cartProduct.Quantity += quantity;
            }

            await cartRepository.SaveChangesAsync();
            return true;
        }
    }
}
