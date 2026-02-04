using TechStore.Data.Models;
using TechStore.Web.ViewModels.Product;

namespace TechStore.Services.Core.Interfaces
{
    public interface IProductService
    {
        Task<ProductDetailsViewModel?> GetProductDetailsViewModelAsync(string? id);
        Task<bool> AddProductAsync(string userId, ProductFormInputModel inputModel);
        Task<ProductsByCategoryViewModel?> GetProductsByCategoryAsync(int categoryId, ProductSort sort);
        Task<ProductFormInputModel?> GetEditableProductByIdAsync(string userId, string? productId);
        Task<bool> EditProductAsync(string userId, ProductFormInputModel inputModel);
        Task<DeleteProductViewModel?> GetProductForDeleteAsync(string? productId, string userId);
        Task<bool> SoftDeleteProductAsync(string userId, DeleteProductViewModel inputModel);
        Task<IEnumerable<ProductInCategoryViewModel?>> SearchByKeywordAsync(string keyword);
        Task<bool> ExistsByNameAsync(string name, string? productIdToSkip);
        Task<Product?> GetDeletedProductByNameAsync(string name);
        Task<ProductFormInputModel?> GetProductForRestoreByIdAsync(string id);
        Task<bool> RestoreByIdAsync(string id);
        Task<Product?> GetProductByIdAsync(string id);
        Task<IEnumerable<ProductManageViewModel>> GetPagedAsync(int page, int pageSize);
        Task<int> GetTotalCountAsync();
    }
}
