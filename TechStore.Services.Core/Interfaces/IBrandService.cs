using TechStore.Data.Models;
using TechStore.Web.ViewModels.Brand;
using TechStore.Web.ViewModels.Product;

namespace TechStore.Services.Core.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<AddProductBrandDropDownModel>> GetBrandsDropDownDataAsync();

        Task<BrandDetailsViewModel?> GetBrandDetailsViewModelAsync(int? id);

        Task<bool> AddBrandAsync(string userId, BrandFormInputViewModel inputModel);

        Task<BrandFormInputViewModel?> GetEditableBrandByIdAsync(string userId, int? brandId);

        Task<bool> EditBrandAsync(string userId, BrandFormInputViewModel inputModel);

        Task<DeleteBrandViewModel?> GetBrandForDeleteByIdAsync(string userId, int? brandId);

        Task<bool> SoftDeleteBrandAsync(string userId, DeleteBrandViewModel deleteModel);

        Task<bool> ExistsByNameAsync(string name, int brandIdToSkip);

        Task<Brand?> GetDeletedBrandByNameAsync(string name);

        Task<BrandFormInputViewModel?> GetBrandForRestoreByIdAsync(int id);

        Task<bool> RestoreByIdAsync(int id);

        Task<IEnumerable<BrandManageViewModel>> GetPagedAsync(int page, int pageSize);

        Task<int> GetTotalCountAsync();
    }
}
