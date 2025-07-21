using TechStore.Web.ViewModels.Product;

namespace TechStore.Services.Core.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<AddProductBrandDropDownModel>> GetBrandsDropDownDataAsync();

    }
}
