using TechStore.Data.Models;
using TechStore.Web.ViewModels.Product;

namespace TechStore.Services.Core.Interfaces
{
    public interface IProductService
    {
        Task<ProductDetailsViewModel?> GetProductDetailsViewModelAsync(Guid productId);
        Task AddProductAsync(ProductFormInputModel inputModel);
        Task<ProductsByCategoryViewModel?> GetProductsByCategoryAsync(int categoryId, ProductSort sort);
        Task<ProductFormInputModel?> GetEditableProductByIdAsync(Guid productId);
        Task<bool> EditProductAsync(ProductFormInputModel inputModel);
        Task<DeleteProductViewModel?> GetProductForDeleteAsync(Guid productId);
        Task<bool> SoftDeleteProductAsync(DeleteProductViewModel inputModel);
        Task<IEnumerable<ProductInCategoryViewModel>> SearchByKeywordAsync(string keyword);
        Task<bool> ExistsByNameAsync(string name, string? productIdToSkip);
        Task<Product?> GetDeletedProductByNameAsync(string name);
        Task<ProductFormInputModel?> GetProductForRestoreByIdAsync(Guid productId);
        Task<bool> RestoreByIdAsync(Guid productId);
        Task<ProductManageListViewModel> GetPagedAsync(int page, int pageSize);
    }
}
