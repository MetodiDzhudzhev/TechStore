using TechStore.Web.ViewModels.Product;

namespace TechStore.Services.Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductInCategoryViewModel>> GetAllProductsByCategoryIdAsync(int categoryId);

        Task<ProductDetailsViewModel?> GetProductDetailsViewModelAsync(string? id);
        Task<bool> AddProductAsync(string userId, ProductFormInputModel inputModel);

    }
}
