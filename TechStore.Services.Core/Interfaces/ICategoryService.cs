using TechStore.Web.ViewModels.Category;

namespace TechStore.Services.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<AllCategoriesIndexViewModel>> GetAllCategoriesAsync();
        Task AddCategoryAsync(CategoryFormInputViewModel inputModel);
    }
}
