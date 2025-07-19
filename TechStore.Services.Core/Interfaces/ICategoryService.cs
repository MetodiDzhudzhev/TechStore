using TechStore.Web.ViewModels.Category;

namespace TechStore.Services.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<AllCategoriesIndexViewModel>> GetAllCategoriesAsync();
        Task AddCategoryAsync(CategoryFormInputViewModel inputModel);

        Task<CategoryFormEditViewModel?> GetEditableCategoryByIdAsync(int? id);

        Task<bool> EditCategoryAsync(CategoryFormEditViewModel inputModel);

        Task<DeleteCategoryViewModel?> GetCategoryForDeleteByIdAsync(int id);

        Task<bool> SoftDeleteCategoryAsync(int id);
    }
}
