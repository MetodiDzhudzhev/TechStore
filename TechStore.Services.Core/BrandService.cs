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
                    brandModel = MapToBrandDetailsViewModel(brand);
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

        public async Task<bool> AddBrandAsync(BrandFormInputViewModel inputModel)
        {
            if (string.IsNullOrWhiteSpace(inputModel.Name))
            {
                return false;
            }

            Brand brand = new Brand
            {
                Name = inputModel.Name.Trim(),
                LogoUrl = inputModel.LogoUrl ?? DefaultImageUrl,
                Description = inputModel.Description,
            };

            await this.brandRepository.AddAsync(brand);
            await this.brandRepository.SaveChangesAsync();

            return true;
        }

        public async Task<BrandFormInputViewModel?> GetEditableBrandByIdAsync(int? brandId)
        {

            if (brandId == null || brandId <= 0)
            {
                return null;
            }

            BrandFormInputViewModel? editableBrand = null;

            Brand? brand = await this.brandRepository
                .GetAllAttached()
                .AsNoTracking()
                .SingleOrDefaultAsync(b => b.Id == brandId);

            if (brand == null)
            {
                return null;
            }

            editableBrand = MapToBrandFormInputViewModel(brand);

            return editableBrand;
        }

        public async Task<bool> EditBrandAsync(BrandFormInputViewModel inputModel)
        {
            bool result = false;

            Brand? editableBrand = await this.brandRepository
            .GetByIdAsync(inputModel.Id);

            if (editableBrand != null)
            {
                editableBrand.Name = inputModel.Name.Trim();
                editableBrand.LogoUrl = inputModel.LogoUrl ?? DefaultImageUrl;
                editableBrand.Description = inputModel.Description;

                this.brandRepository.Update(editableBrand);
                await this.brandRepository.SaveChangesAsync();

                result = true;
            }

            return result;
        }

        public async Task<DeleteBrandViewModel?> GetBrandForDeleteByIdAsync(int? brandId)
        {
            DeleteBrandViewModel? brandForDeleteViewModel = null;

            if (brandId != null)
            {
                Brand? brand = await this.brandRepository
                    .GetAllAttached()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(b => b.Id == brandId);

                if (brand != null && brand.IsDeleted == false)
                {
                    brandForDeleteViewModel = MapToDeleteBrandViewModel(brand);
                }
            }

            return brandForDeleteViewModel;
        }

        public async Task<bool> SoftDeleteBrandAsync(DeleteBrandViewModel deleteModel)
        {
            bool result = false;

            Brand? brandToDelete = await this.brandRepository
                .GetByIdAsync(deleteModel.Id);

            if (brandToDelete != null)
            {
                this.brandRepository.Delete(brandToDelete);
                await this.brandRepository.SaveChangesAsync();
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

            BrandFormInputViewModel modelToRestore = MapToBrandFormInputViewModel(brand);

            return modelToRestore;
        }

        public async Task<bool> RestoreByIdAsync(int id)
        {
            Brand? brand = await this.brandRepository
                .GetAllAttached()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted);

            if (brand == null)
            {
                return false;
            }

            brand.IsDeleted = false;

            this.brandRepository.Update(brand);
            await this.brandRepository.SaveChangesAsync();
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

        public async Task<IEnumerable<BrandManageViewModel>> GetAllAsync()
        {
            return await this.brandRepository
                .GetAllAttached()
                .AsNoTracking()
                .OrderBy(b => b.Name)
                .Select(b => new BrandManageViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    LogoUrl = b.LogoUrl ?? DefaultImageUrl,
                    Description = b.Description,
                })
                .ToListAsync();
        }

        private static BrandDetailsViewModel MapToBrandDetailsViewModel(Brand brand)
        {
            BrandDetailsViewModel viewModel = new BrandDetailsViewModel()
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                logoUrl = brand.LogoUrl,
            };

            return viewModel;
        }

        private static BrandFormInputViewModel MapToBrandFormInputViewModel(Brand brand)
        {
            BrandFormInputViewModel viewModel = new BrandFormInputViewModel()
            {
                Id = brand.Id,
                Name = brand.Name,
                LogoUrl = brand.LogoUrl ?? DefaultImageUrl,
                Description = brand.Description,
            };

            return viewModel;
        }

        private static DeleteBrandViewModel MapToDeleteBrandViewModel(Brand brand)
        {
            DeleteBrandViewModel viewModel = new DeleteBrandViewModel()
            {
                Id = brand.Id,
                Name = brand.Name,
                LogoUrl = brand.LogoUrl,
                Description = brand.Description,
            };

            return viewModel;
        }
    }
}