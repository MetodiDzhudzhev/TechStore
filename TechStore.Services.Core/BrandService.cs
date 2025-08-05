using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Brand;
using TechStore.Web.ViewModels.Product;
using static TechStore.GCommon.ApplicationConstants;

namespace TechStore.Services.Core
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository brandRepository;
        private readonly IUserRepository userRepository;

        public BrandService(IBrandRepository brandRepository, IUserRepository userRepository)
        {
            this.brandRepository = brandRepository;
            this.userRepository = userRepository;
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

        public async Task<bool> AddBrandAsync(string userId, BrandFormInputViewModel inputModel)
        {
            bool result = false;

            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            if (user != null)
            {
                Brand brand = new Brand()
                {
                    Name = inputModel.Name.Trim(),
                    LogoUrl = inputModel.LogoUrl ?? DefaultImageUrl,
                    Description = inputModel.Description,
                };

                await this.brandRepository.AddAsync(brand);
                result = true;
            }

            return result;
        }

        public async Task<BrandFormInputViewModel?> GetBrandForRestoreByIdAsync(int id)
        {
            Brand? brand = await this.brandRepository
                .GetAllAttached()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
            {
                return null;
            }

            BrandFormInputViewModel modelToRestore = new BrandFormInputViewModel()
            {
                Id = brand.Id,
                Name = brand.Name,
                LogoUrl = brand.LogoUrl ?? DefaultImageUrl,
                Description = brand.Description,
            };

            return modelToRestore;
        }

        public async Task<bool> RestoreByIdAsync(int id)
        {
            Brand? brand = await this.brandRepository
                .GetAllAttached()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted);

            if (brand == null)
            {
                return false;
            }

            brand.IsDeleted = false;

            await this.brandRepository.UpdateAsync(brand);
            return true;
        }

        public async Task<bool> ExistsByNameAsync(string name, int brandIdToSkip)
        {
            return await this.brandRepository.ExistsByNameAsync(name, brandIdToSkip);
        }

        public async Task<Brand?> GetDeletedBrandByNameAsync(string name)
        {
            return await this.brandRepository.GetDeletedBrandByNameAsync(name);
        }

        public async Task<IEnumerable<BrandManageViewModel>> GetPagedAsync(int page, int pageSize)
        {
            IEnumerable<BrandManageViewModel> brands = await this.brandRepository
                .GetAllAttached()
                .OrderBy(b => b.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BrandManageViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    LogoUrl = b.LogoUrl ?? DefaultImageUrl,
                    Description = b.Description,
                })
                .ToListAsync();

            return brands;
        }
        public async Task<int> GetTotalCountAsync()
        {
            int countOfBrands = await this.brandRepository
                .CountAsync();

            return countOfBrands;
        }
    }
}
