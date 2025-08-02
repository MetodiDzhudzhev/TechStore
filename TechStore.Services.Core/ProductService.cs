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


        public async Task<IEnumerable<ProductInCategoryViewModel?>> GetAllProductsByCategoryIdAsync(int categoryId)
        {
            Category? Category = await this.categoryRepository.GetByIdAsync(categoryId);

            if (Category == null)
            {
                return null;
            }

            IEnumerable<Product> products = await this.productRepository.GetByCategoryAsync(categoryId);

            IEnumerable<ProductInCategoryViewModel> result = products
                .OrderBy(p => p.Name)
                .Select(p => new ProductInCategoryViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl ?? DefaultImageUrl,
                    Price = p.Price,
                    QuantityInStock = p.QuantityInStock
                })
                .ToList();

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
                        ImageUrl = currentProduct.ImageUrl ?? DefaultImageUrl,
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
                    Name = inputModel.Name.Trim(),
                    Description = inputModel.Description,
                    ImageUrl = inputModel.ImageUrl ?? DefaultImageUrl,
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

        public async Task<ProductFormInputModel?> GetEditableProductByIdAsync(string userId, string? id)
        {
            ProductFormInputModel? model = null;

            bool isIdValid = Guid.TryParse(id, out Guid productId);

            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            if (user != null && isIdValid)
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
            bool result = false;

            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            bool categoryExists = await this.categoryRepository
                    .ExistsAsync(inputModel.CategoryId);

            bool brandExists = await this.brandRepository
                        .ExistsAsync(inputModel.BrandId);

            if ((user != null) && categoryExists && brandExists)
            {
                Product? product = await this.productRepository.GetByIdAsync(Guid.Parse(inputModel.Id));

                if (product != null)
                {
                    product.Name = inputModel.Name.Trim();
                    product.Description = inputModel.Description;
                    product.ImageUrl = inputModel.ImageUrl ?? DefaultImageUrl;
                    product.Price = inputModel.Price;
                    product.QuantityInStock = inputModel.QuantityInStock;
                    product.CategoryId = inputModel.CategoryId;
                    product.BrandId = inputModel.BrandId;

                    await this.productRepository.UpdateAsync(product);
                    result = true;
                }
            }

            return result;
        }

        public async Task<DeleteProductViewModel?> GetProductForDeleteAsync(string? productId, string userId)
        {
            bool isProductIdValid = Guid.TryParse(productId, out Guid productForDeleteId);

            if (!isProductIdValid)
            {
                return null;
            }

            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            DeleteProductViewModel? modelForDelete = null;

            if (user != null)
            {
                Product? product = await this.productRepository
                            .GetAllAttached()
                            .AsNoTracking()
                            .Include(p => p.OrdersProducts)
                            .ThenInclude(op => op.Order)
                            .SingleOrDefaultAsync(p => p.Id == productForDeleteId);

                if (product != null && product.IsDeleted == false)
                {
                    bool hasPendingOrders = product.OrdersProducts
                        .Any(op => op.Order.Status == 0);

                    if (!hasPendingOrders)
                    {
                        modelForDelete = new DeleteProductViewModel
                        {
                            Id = product.Id.ToString(),
                            Name = product.Name,
                            CategoryId = product.CategoryId,
                            ImageUrl = product.ImageUrl ?? DefaultImageUrl,
                        };
                    }

                }
            }

            return modelForDelete;
        }

        public async Task<bool> SoftDeleteProductAsync(string userId, DeleteProductViewModel inputModel)
        {
            bool result = false;

            bool isProductIdValid = Guid.TryParse(inputModel.Id, out Guid productId);

            if (!isProductIdValid)
            {
                return result;
            }

            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            if (user != null)
            {
                Product? product = await this.productRepository.GetByIdAsync(productId);

                if (product != null && product.IsDeleted == false)
                {
                    bool hasPendingOrders = product.OrdersProducts
                        .Any(op => op.Order.Status == 0);

                    if (!hasPendingOrders)
                    {
                        await this.productRepository.DeleteAsync(product);

                        result = true;
                    }
                }
            }

            return result;
        }

        public async Task<bool> ExistsByNameAsync(string name, string? productIdToSkip)
        {
            return await this.productRepository.ExistsByNameAsync(name, productIdToSkip);
        }

        public async Task<Product?> GetDeletedProductByNameAsync(string name)
        {
            return await this.productRepository.GetDeletedProductByNameAsync(name);
        }

        public async Task<ProductFormInputModel?> GetProductForRestoreByIdAsync(string id)
        {
            var isIdValid = Guid.TryParse(id, out Guid productId);

            if (!isIdValid)
            {
                return null;
            }

            Product? product = await this.productRepository
                .GetAllAttached()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return null;
            }

            ProductFormInputModel modelToRestore = new ProductFormInputModel()
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl ?? DefaultImageUrl,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId
            };

            return modelToRestore;
        }

        public async Task<bool> RestoreByIdAsync(string id)
        {
            var isIdValid = Guid.TryParse(id, out Guid productId);

            if (!isIdValid)
            {
                return false;
            }

            Product? product = await this.productRepository
                .GetAllAttached()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsDeleted);

            if (product == null)
            {
                return false;
            }

            product.IsDeleted = false;

            await this.productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            return await this.productRepository
                .GetAllAttached()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));
        }
    }
}
