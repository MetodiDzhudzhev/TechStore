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

        public async Task<bool> AddProductAsync(string cartId, string? productId)
        {
            Cart? cart = await GetValidCartAsync(cartId);

            if (cart == null)
            {
                return false;
            }

            Product? product = await GetValidProductAsync(productId);

            if (product == null)
            {
                return false;
            }

            CartProduct? cartProduct = cart.Products.SingleOrDefault(cp => cp.ProductId == product.Id);

            int currentQuantityInCart = cartProduct?.Quantity ?? 0;

            if (currentQuantityInCart + 1 > product.QuantityInStock)
            {
                return false;
            }

            if (cartProduct == null)
            {
                cart.Products.Add(new CartProduct
                {
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = 1
                });

                await cartRepository.SaveChangesAsync();
                return true;
            }
            else
            {
                return await IncreaseProductQuantityAsync(cartId, productId);
            }
        }

        public async Task<bool> IncreaseProductQuantityAsync(string cartId, string? productId)
        {
            Cart? cart = await GetValidCartAsync(cartId);

            if (cart == null)
            {
                return false;
            }

            Product? product = await GetValidProductAsync(productId);

            if (product == null)
            {
                return false;
            }

            CartProduct? cartProduct = cart.Products.SingleOrDefault(cp => cp.ProductId == product.Id);

            if (cartProduct == null)
            {
                return await AddProductAsync(cartId, productId);
            }

            if (cartProduct.Quantity + 1 > product.QuantityInStock)
            {
                return false;
            }

            cartProduct.Quantity += 1;
            await cartRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveProductAsync(string cartId, string? productId)
        {
            Cart? cart = await GetValidCartAsync(cartId);

            if (cart == null)
            {
                return false;
            }

            Product? product = await GetValidProductAsync(productId);

            if (product == null)
            {
                return false;
            }

            CartProduct? cartProduct = cart.Products.SingleOrDefault(cp => cp.ProductId == product.Id);

            if (cartProduct == null)
            {
                return false;
            }
            else
            {
                cart.Products.Remove(cartProduct);
                await cartRepository.SaveChangesAsync();

                return true;
            }
        }

        public async Task<bool> DecreaseProductAsync(string cartId, string? productId)
        {
            Cart? cart = await GetValidCartAsync(cartId);

            if (cart == null)
            {
                return false;
            }

            Product? product = await GetValidProductAsync(productId);

            if (product == null)
            {
                return false;
            }

            CartProduct? cartProduct = cart.Products.SingleOrDefault(cp => cp.ProductId == product.Id);

            if (cartProduct == null)
            {
                return false;
            }
            else
            {
                if (cartProduct.Quantity > 1)
                {
                    cartProduct.Quantity -= 1;
                }
                else
                {
                    cartProduct.Quantity = 1;
                }

                await cartRepository.SaveChangesAsync();
                return true;
            }
        }



        private static Guid? ParseGuidOrNull(string? id)
        {
            return Guid.TryParse(id, out Guid result) ? result : null;
        }

        private async Task<Cart?> GetValidCartAsync(string? cartId)
        {
            Guid? validCartId = ParseGuidOrNull(cartId);

            if (validCartId == null)
            {
                return null;
            }

            Cart? cart = await cartRepository.GetCartWithProductsAsync(validCartId.Value);

            return cart;
        }

        private async Task<Product?> GetValidProductAsync(string? productId)
        {
            Guid? validProductId = ParseGuidOrNull(productId);

            if (validProductId == null)
            {
                return null;
            }

            Product? product = await productRepository.GetByIdAsync(validProductId.Value);

            return product;
        }

    }
}