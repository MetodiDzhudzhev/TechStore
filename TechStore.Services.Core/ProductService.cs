using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Product;
using static TechStore.GCommon.ApplicationConstants;

namespace TechStore.Services.Core
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<IEnumerable<AllProductsByCategoryIdViewModel>> GetAllProductsByCategoryIdAsync(int categoryId)
        {
            IEnumerable<Product> products = await this.productRepository.GetByCategoryAsync(categoryId);

            IEnumerable<AllProductsByCategoryIdViewModel> result = products
                .Select(p => new AllProductsByCategoryIdViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    QuantityInStock = p.QuantityInStock
                })
                .ToList();

            foreach (AllProductsByCategoryIdViewModel product in result)
            {
                if (String.IsNullOrEmpty(product.ImageUrl))
                {
                    product.ImageUrl = DefaultImageUrl;
                }
            }

            return result;
        }

        public async Task<ProductDetailsViewModel?> GetProductDetailsViewModelAsync(string? id)
        {

            bool isIdValid = Guid.TryParse(id, out Guid productId);

            if (isIdValid)
            {

                Product? currentProduct = await this.productRepository
                        .GetAllAttached()
                        .Include(p => p.Brand)
                        .FirstOrDefaultAsync(p => p.Id == productId);

                if (currentProduct != null)
                {
                    ProductDetailsViewModel viewModel = new ProductDetailsViewModel
                    {

                        Id = currentProduct.Id,
                        Name = currentProduct.Name,
                        Brand = currentProduct.Brand.Name,
                        Description = currentProduct.Description,
                        ImageUrl = string.IsNullOrEmpty(currentProduct.ImageUrl)
                                                        ? DefaultImageUrl
                                                        : currentProduct.ImageUrl,
                        Price = currentProduct.Price,
                        QuantityInStock = currentProduct.QuantityInStock,
                        CategoryId = currentProduct.CategoryId,
                    };

                    return viewModel;
                }
            }

            return null;
        }
    }
}
