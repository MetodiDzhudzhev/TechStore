using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Brand;
using TechStore.Web.ViewModels.Product;

namespace TechStore.Services.Core
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository brandRepository;

        public BrandService(IBrandRepository brandRepository)
        {
            this.brandRepository = brandRepository;
        }

        public async Task<BrandDetailsViewModel?> GetBrandDetailsViewModelAsync(int? id)
        {
            BrandDetailsViewModel? brandModel = null;

            if (id != null)
            {
                Brand? brand = await this.brandRepository
                    .GetAllAttached()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(b => b.Id == id);

                if (brand != null)
                {
                    brandModel = new BrandDetailsViewModel
                    {
                        Id = brand.Id,
                        Name = brand.Name,
                        Description = brand.Description,
                        logoUrl = brand.LogoUrl,
                    };
                }
            }

            return brandModel;
        }

        public async Task<IEnumerable<AddProductBrandDropDownModel>> GetBrandsDropDownDataAsync()
        {
            IEnumerable<AddProductBrandDropDownModel> brandsAsDropDown = await this.brandRepository
                .GetAllAttached()
                .AsNoTracking()
                .Select(b => new AddProductBrandDropDownModel()
                {
                    Id = b.Id,
                    Name = b.Name,
                })
                .ToListAsync();

            return brandsAsDropDown;
        }
    }
}
