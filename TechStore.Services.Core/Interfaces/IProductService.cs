using TechStore.Web.ViewModels.Product;

namespace TechStore.Services.Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductInCategoryViewModel?>> GetAllProductsByCategoryIdAsync(int categoryId);

        Task<ProductDetailsViewModel?> GetProductDetailsViewModelAsync(string? id);
        Task<bool> AddProductAsync(string userId, ProductFormInputModel inputModel);
        Task<ProductsByCategoryViewModel?> GetProductsByCategoryAsync(int categoryId);

        Task<ProductFormInputModel?> GetEditableProductByIdAsync(string? id);

        Task<bool> EditProductAsync(string userId, ProductFormInputModel inputModel);
    }
}
