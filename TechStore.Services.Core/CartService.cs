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

        public async Task<CartViewModel?> GetCartAsync(Guid cartId)
        {
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

        public async Task<bool> AddProductAsync(Guid cartId, Guid productId, int quantity)
        {
            return await AddQuantityInternalAsync(cartId, productId, quantity);
        }

        public async Task<bool> IncreaseProductQuantityAsync(Guid cartId, Guid productId)
        {
            return await AddQuantityInternalAsync(cartId, productId, 1);

        }

        public async Task<bool> RemoveProductAsync(Guid cartId, Guid productId)
        {
            Cart? cart = await GetCartAsyncInternal(cartId);

            if (cart == null)
            {
                return false;
            }

            Product? product = await GetProductAsyncInternal(productId);

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

        public async Task<bool> DecreaseProductAsync(Guid cartId, Guid productId)
        {
            Cart? cart = await GetCartAsyncInternal(cartId);

            if (cart == null)
            {
                return false;
            }

            Product? product = await GetProductAsyncInternal(productId);

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

        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            Cart? cart = await GetCartAsyncInternal(cartId);

            if (cart == null)
            {
                return false;
            }

            cart.Products.Clear();

            await cartRepository.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCartItemsCountAsync(Guid cartId)
        {
            return await cartRepository.GetCartItemsCountAsync(cartId);
        }

        private async Task<bool> AddQuantityInternalAsync(Guid cartId, Guid productId, int quantityToAdd)
        {
            if (quantityToAdd < 1)
            {
                return false;
            }

            Cart? cart = await GetCartAsyncInternal(cartId);

            if (cart == null)
            {
                return false;
            }

            Product? product = await GetProductAsyncInternal(productId);

            if (product == null)
            {
                return false;
            }

            CartProduct? cartProduct = cart.Products
                .SingleOrDefault(cp => cp.ProductId == product.Id);

            int currentQuantity = cartProduct?.Quantity ?? 0;

            if (currentQuantity + quantityToAdd > product.QuantityInStock)
            {
                return false;
            }

            if (cartProduct == null)
            {
                cart.Products.Add(new CartProduct
                {
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = quantityToAdd
                });
            }
            else
            {
                cartProduct.Quantity += quantityToAdd;
            }

            await cartRepository.SaveChangesAsync();
            return true;
        }

        private async Task<Cart?> GetCartAsyncInternal(Guid cartId)
        {
            return await cartRepository.GetCartWithProductsAsync(cartId);
        }

        private async Task<Product?> GetProductAsyncInternal(Guid productId)
        {
            return await productRepository.GetByIdAsync(productId);
        }
    }
}