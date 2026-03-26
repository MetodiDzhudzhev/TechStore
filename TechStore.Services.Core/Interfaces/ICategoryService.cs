using TechStore.Data.Models;
using TechStore.Web.ViewModels.Category;
using TechStore.Web.ViewModels.Product;

namespace TechStore.Services.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<AllCategoriesIndexViewModel>> GetAllCategoriesAsync();
        Task AddCategoryAsync(CategoryFormInputViewModel inputModel);
        Task<CategoryFormInputViewModel?> GetEditableCategoryByIdAsync(int? categoryId);
        Task<bool> EditCategoryAsync(CategoryFormInputViewModel inputModel);
        Task<DeleteCategoryViewModel?> GetCategoryForDeleteByIdAsync(int? categoryId);
        Task<bool> SoftDeleteCategoryAsync(DeleteCategoryViewModel deleteModel);
        Task<IEnumerable<AddProductCategoryDropDownModel>> GetCategoriesDropDownDataAsync();
        Task<bool> ExistsByNameAsync(string name, int categoryIdToSkip);
        Task<bool> ExistsAsync(int id);
        Task<Category?> GetDeletedCategoryByNameAsync(string name);
        Task<CategoryFormInputViewModel?> GetCategoryForRestoreByIdAsync(int id);
        Task<bool> RestoreByIdAsync(int id);
        Task<IEnumerable<CategoryManageViewModel>> GetAllAsync();
    }
}
