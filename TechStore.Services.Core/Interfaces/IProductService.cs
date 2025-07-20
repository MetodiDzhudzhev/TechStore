using TechStore.Web.ViewModels.Product;

namespace TechStore.Services.Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<AllProductsByCategoryIdViewModel>> GetAllProductsByCategoryIdAsync(int categoryId);

        Task<ProductDetailsViewModel?> GetProductDetailsViewModelAsync(string? id);
    }
}
