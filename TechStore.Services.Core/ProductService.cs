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

        public ProductService(IProductRepository productRepository,
                            ICategoryRepository categoryRepository)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
        }

        public async Task<ProductsByCategoryViewModel?> GetProductsByCategoryAsync(int categoryId, ProductSort sort)
        {
            Category? category = await this.categoryRepository.GetByIdAsync(categoryId);

            if (category == null)
            {
                return null;
            }

            IQueryable<Product> query = productRepository.GetByCategoryQuery(categoryId);

            switch (sort)
            {
                case ProductSort.PriceAsc:
                    query = query.OrderBy(p => p.Price);
                    break;

                case ProductSort.PriceDesc:
                    query = query.OrderByDescending(p => p.Price);
                    break;

                default:
                    query = query.OrderBy(p => p.Name);
                    break;
            }

            List<ProductInCategoryViewModel> products = await query
                .Select(p => new ProductInCategoryViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl ?? DefaultImageUrl,
                    Price = p.Price,
                    QuantityInStock = p.QuantityInStock
                })
                .ToListAsync();

            return new ProductsByCategoryViewModel
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Products = products,
                Sort = sort
            };
        }

        public async Task<ProductDetailsViewModel?> GetProductDetailsViewModelAsync(Guid productId)
        {
            Product? currentProduct = await this.productRepository
                    .GetAllAttached()
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.Id == productId);

            if (currentProduct != null)
            {
                ProductDetailsViewModel viewModel = MapToProductDetailsViewModel(currentProduct);

                return viewModel;
            }

            return null;
        }
        public async Task AddProductAsync(ProductFormInputModel inputModel)
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
            await this.productRepository.SaveChangesAsync();
        }

        public async Task<ProductFormInputModel?> GetEditableProductByIdAsync(Guid productId)
        {
            ProductFormInputModel? model = null;

            Product? product = await this.productRepository.GetByIdAsync(productId);

            if (product != null)
            {
                model = MapToProductFormInputModel(product);
            }

            return model;
        }

        public async Task<bool> EditProductAsync(ProductFormInputModel inputModel)
        {
            bool result = false;

            Product? product = await this.productRepository.GetByIdAsync(inputModel.Id);

            if (product != null)
            {
                product.Name = inputModel.Name.Trim();
                product.Description = inputModel.Description;
                product.ImageUrl = inputModel.ImageUrl ?? DefaultImageUrl;
                product.Price = inputModel.Price;
                product.QuantityInStock = inputModel.QuantityInStock;
                product.CategoryId = inputModel.CategoryId;
                product.BrandId = inputModel.BrandId;

                this.productRepository.Update(product);
                await this.productRepository.SaveChangesAsync();

                result = true;
            }

            return result;
        }

        public async Task<DeleteProductViewModel?> GetProductForDeleteAsync(Guid productId)
        {
            DeleteProductViewModel? modelForDelete = null;

            Product? product = await this.productRepository
                        .GetAllAttached()
                        .AsNoTracking()
                        .Include(p => p.OrdersProducts)
                        .ThenInclude(op => op.Order)
                        .SingleOrDefaultAsync(p => p.Id == productId);

            if (product != null && product.IsDeleted == false)
            {
                bool hasPendingOrders = HasPendingOrders(product);

                if (!hasPendingOrders)
                {
                    modelForDelete = MapToDeleteProductViewModel(product);
                }
            }

            return modelForDelete;
        }

        public async Task<bool> SoftDeleteProductAsync(DeleteProductViewModel inputModel)
        {
            bool result = false;

            Product?  product = await this.productRepository
                .GetAllAttached()
                .Include(p => p.OrdersProducts)
                .ThenInclude(op => op.Order)
                .FirstOrDefaultAsync(p => p.Id == inputModel.Id);

            if (product != null && product.IsDeleted == false)
            {
                bool hasPendingOrders = HasPendingOrders(product);

                if (!hasPendingOrders)
                {
                    this.productRepository.Delete(product);
                    await this.productRepository.SaveChangesAsync();

                    result = true;
                }
            }

            return result;
        }

        public async Task<IEnumerable<ProductInCategoryViewModel>> SearchByKeywordAsync(string keyword)
        {
            IEnumerable<Product> products = await productRepository.SearchByKeywordAsync(keyword);

            return products
                .OrderBy(p => p.Name)
                .Select(p => new ProductInCategoryViewModel
                {
                   Id = p.Id,
                   Name = p.Name,
                   ImageUrl = p.ImageUrl ?? DefaultImageUrl,
                   Price = p.Price,
                   QuantityInStock = p.QuantityInStock
                });
        }

        public async Task<bool> ExistsByNameAsync(string name, string? productIdToSkip)
        {
            return await this.productRepository.ExistsByNameAsync(name, productIdToSkip);
        }

        public async Task<Product?> GetDeletedProductByNameAsync(string name)
        {
            return await this.productRepository.GetDeletedProductByNameAsync(name);
        }

        public async Task<ProductFormInputModel?> GetProductForRestoreByIdAsync(Guid productId)
        {
            Product? product = await this.productRepository
                .GetAllAttached()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return null;
            }

            ProductFormInputModel modelToRestore = MapToProductFormInputModel(product);

            return modelToRestore;
        }

        public async Task<bool> RestoreByIdAsync(Guid productId)
        {
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

            this.productRepository.Update(product);
            await this.productRepository.SaveChangesAsync();
            return true;
        }

        public async Task<ProductManageListViewModel> GetPagedAsync(int page, int pageSize)
        {
            (page, pageSize) = NormalizePaging(page, pageSize);


            int totalCount = await productRepository.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (totalPages == 0)
            {
                return new ProductManageListViewModel
                {
                    Products = new List<ProductManageViewModel>(),
                    CurrentPage = 1,
                    TotalPages = 0
                };
            }

            if (page > totalPages)
            {
                page = totalPages;
            }

            IEnumerable<ProductManageViewModel> products = await this.productRepository
                .GetAllAttached()
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductManageViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CategoryName = p.Category.Name,
                    BrandName = p.Brand.Name,
                    Price = p.Price,
                    QuantityInStock = p.QuantityInStock,
                    ImageUrl = p.ImageUrl ?? DefaultImageUrl,
                })
                .ToListAsync();

            return new ProductManageListViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
            };
        }

        private static (int page, int pageSize) NormalizePaging(int page, int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 5;
            }

            return (page, pageSize);
        }

        private static bool HasPendingOrders(Product product)
        {
            return product.OrdersProducts.Any(op => op.Order.Status == 0);
        }

        private static ProductDetailsViewModel MapToProductDetailsViewModel(Product product)
        {
            return new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                BrandId = product.BrandId,
                Brand = product.Brand.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl ?? DefaultImageUrl,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                CategoryId = product.CategoryId,
            };
        }

        private static ProductFormInputModel MapToProductFormInputModel(Product product)
        {
            return new ProductFormInputModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId
            };
        }

        private static DeleteProductViewModel MapToDeleteProductViewModel(Product product)
        {
            return new DeleteProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl ?? DefaultImageUrl,
            };
        }
    }
}