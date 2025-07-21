using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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


        public async Task<IEnumerable<ProductInCategoryViewModel?>> GetAllProductsByCategoryIdAsync(int categoryId)
        {
            Category? Category = await this.categoryRepository.GetByIdAsync(categoryId);

            if (Category == null)
            {
                return null;
            }

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

        public async Task<ProductsByCategoryViewModel?> GetProductsByCategoryAsync(int categoryId)
        {
            Category? category = await this.categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return null;
            }

            IEnumerable<ProductInCategoryViewModel?> products = await this.GetAllProductsByCategoryIdAsync(categoryId);

            ProductsByCategoryViewModel viewModel = new ProductsByCategoryViewModel
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Products = products
            };

            return viewModel;
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
            bool result = false;
            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            bool categoryExists = await this.categoryRepository
                    .ExistsAsync(inputModel.CategoryId);

            bool brandExists = await this.brandRepository
                        .ExistsAsync(inputModel.BrandId);

            if ((user != null) && categoryExists && brandExists)
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
                result = true;
            }

            return result;
        }

        public async Task<ProductFormInputModel?> GetEditableProductByIdAsync(string? id)
        {
            ProductFormInputModel? model = null;

            bool isIdValid = Guid.TryParse(id, out Guid productId);

            if (isIdValid)
            {
                Product? product = await this.productRepository.GetByIdAsync(productId);

                if (product != null)
                {
                    model = new ProductFormInputModel
                    {
                        Id = product.Id.ToString(),
                        Name = product.Name,
                        Description = product.Description,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        QuantityInStock = product.QuantityInStock,
                        CategoryId = product.CategoryId,
                        BrandId = product.BrandId
                    };
                }
            }

            return model;
        }

        public async Task<bool> EditProductAsync(string userId, ProductFormInputModel inputModel)
        {
            Product? product = await this.productRepository.GetByIdAsync(Guid.Parse(inputModel.Id));

            if (product != null)
            {
                product.Name = inputModel.Name;
                product.Description = inputModel.Description;
                product.ImageUrl = string.IsNullOrWhiteSpace(inputModel.ImageUrl)
                                                            ? DefaultImageUrl
                                                            : inputModel.ImageUrl;
                product.Price = inputModel.Price;
                product.QuantityInStock = inputModel.QuantityInStock;
                product.CategoryId = inputModel.CategoryId;
                product.BrandId = inputModel.BrandId;

                await this.productRepository.UpdateAsync(product);
                return true;
            }

            return false;
        }
    }
}
