using TechStore.Data.Models;
using TechStore.Web.ViewModels.Category;
using TechStore.Web.ViewModels.Product;

namespace TechStore.Services.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<AllCategoriesIndexViewModel>> GetAllCategoriesAsync();
        Task<bool> AddCategoryAsync(string userId, CategoryFormInputViewModel inputModel);

        Task<CategoryFormInputViewModel?> GetEditableCategoryByIdAsync(string userId, int? categoryId);

        Task<bool> EditCategoryAsync(string userId, CategoryFormInputViewModel inputModel);

        Task<DeleteCategoryViewModel?> GetCategoryForDeleteByIdAsync(string userId, int? categoryId);

        Task<bool> SoftDeleteCategoryAsync(string userId, DeleteCategoryViewModel deleteModel);

        Task<IEnumerable<AddProductCategoryDropDownModel>> GetCategoriesDropDownDataAsync();

        Task<bool> ExistsByNameAsync(string name, int categoryIdToSkip);

        Task<bool> ExistsAsync(int id);

        Task<Category?> GetDeletedCategoryByNameAsync(string name);

        Task<CategoryFormInputViewModel?> GetCategoryForRestoreByIdAsync(int id);

        Task<bool> RestoreByIdAsync(int id);

        Task<IEnumerable<CategoryManageViewModel>> GetPagedAsync(int page, int pageSize);

        Task<int> GetTotalCountAsync();
    }
}
