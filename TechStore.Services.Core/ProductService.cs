using Microsoft.EntityFrameworkCore;
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
        private readonly ICategoryRepository categoryRepository;
        private readonly IBrandRepository brandRepository;
        private readonly IUserRepository userRepository;

        public ProductService(IProductRepository productRepository,
                            ICategoryRepository categoryRepository,
                             IBrandRepository brandRepository,
                             IUserRepository userRepository)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.brandRepository = brandRepository;
            this.userRepository = userRepository;
        }


        public async Task<IEnumerable<ProductInCategoryViewModel>> GetAllProductsByCategoryIdAsync(int categoryId)
        {
            IEnumerable<Product> products = await this.productRepository.GetByCategoryAsync(categoryId);

            IEnumerable<ProductInCategoryViewModel> result = products
                .Select(p => new ProductInCategoryViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    QuantityInStock = p.QuantityInStock
                })
                .ToList();

            foreach (ProductInCategoryViewModel product in result)
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
                        ImageUrl = string.IsNullOrWhiteSpace(currentProduct.ImageUrl)
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
        public async Task<bool> AddProductAsync(string userId, ProductFormInputModel inputModel)
        {
            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            if (user != null)
            {
                bool categoryExists = await this.categoryRepository
                    .ExistsAsync(inputModel.CategoryId);

                if (categoryExists)
                {
                    bool brandExists = await this.brandRepository
                        .ExistsAsync(inputModel.BrandId);

                    if (brandExists)
                    {
                        Product product = new Product
                        {
                            Name = inputModel.Name,
                            Description = inputModel.Description,
                            ImageUrl = string.IsNullOrWhiteSpace(inputModel.ImageUrl)
                                                ? DefaultImageUrl
                                                : inputModel.ImageUrl,
                            Price = inputModel.Price,
                            QuantityInStock = inputModel.QuantityInStock,
                            CategoryId = inputModel.CategoryId,
                            BrandId = inputModel.BrandId
                        };

                        await this.productRepository.AddAsync(product);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
