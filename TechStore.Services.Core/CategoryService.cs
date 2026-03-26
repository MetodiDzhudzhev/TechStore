using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Category;
using TechStore.Web.ViewModels.Product;
using static TechStore.GCommon.ApplicationConstants;

namespace TechStore.Services.Core
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<AllCategoriesIndexViewModel>> GetAllCategoriesAsync()
        {
            IEnumerable<AllCategoriesIndexViewModel> allCategories = await this.categoryRepository
                .GetAllAttached()
                .AsNoTracking()
                .Select(c => new AllCategoriesIndexViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl ?? DefaultImageUrl,
                })
                .ToListAsync();

            return allCategories;
        }
        public async Task AddCategoryAsync(CategoryFormInputViewModel inputModel)
        {
            Category category = new Category()
            {
                Name = inputModel.Name.Trim(),
                ImageUrl = inputModel.ImageUrl ?? DefaultImageUrl,
            };

            await this.categoryRepository.AddAsync(category);
            await this.categoryRepository.SaveChangesAsync();
        }

        public async Task<CategoryFormInputViewModel?> GetEditableCategoryByIdAsync(int? categoryId)
        {

            if (categoryId == null || categoryId <= 0)
            {
                return null;
            }

            CategoryFormInputViewModel? editableCategory = null;

            Category? category = await this.categoryRepository
                .GetAllAttached()
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return null;
            }

            editableCategory = MapToCategoryFormInputViewModel(category);

            return editableCategory;
        }

        public async Task<bool> EditCategoryAsync(CategoryFormInputViewModel inputModel)
        {
            bool result = false;

            Category? editableCategory = await this.categoryRepository
            .GetByIdAsync(inputModel.Id);

            if (editableCategory != null)
            {
                editableCategory.Name = inputModel.Name.Trim();
                editableCategory.ImageUrl = inputModel.ImageUrl ?? DefaultImageUrl;
                this.categoryRepository.Update(editableCategory);
                await this.categoryRepository.SaveChangesAsync();

                result = true;
            }

            return result;
        }

        public async Task<DeleteCategoryViewModel?> GetCategoryForDeleteByIdAsync(int? categoryId)
        {
            DeleteCategoryViewModel? categoryForDeleteViewModel = null;

            if (categoryId != null)
            {
                Category? category = await this.categoryRepository
                    .GetAllAttached()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(c => c.Id == categoryId);

                if (category != null && category.IsDeleted == false)
                {
                    categoryForDeleteViewModel = MapToDeleteCategoryViewModel(category);
                }
            }

            return categoryForDeleteViewModel;
        }

        public async Task<bool> SoftDeleteCategoryAsync(DeleteCategoryViewModel deleteModel)
        {
            bool result = false;

            Category? categoryToDelete = await this.categoryRepository
                .GetByIdAsync(deleteModel.Id);

            if (categoryToDelete != null)
            {
                this.categoryRepository.Delete(categoryToDelete);
                await this.categoryRepository.SaveChangesAsync();
                result = true;
            }

            return result;
        }

        public async Task<IEnumerable<AddProductCategoryDropDownModel>> GetCategoriesDropDownDataAsync()
        {
            IEnumerable<AddProductCategoryDropDownModel> categoriesAsDropDown = await this.categoryRepository
                .GetAllAttached()
                .AsNoTracking()
                .Select(c => new AddProductCategoryDropDownModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToListAsync();

            return categoriesAsDropDown;
        }

        public async Task<bool> ExistsByNameAsync(string name, int categoryIdToSkip)
        {
            return await this.categoryRepository.ExistsByNameAsync(name, categoryIdToSkip);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await this.categoryRepository.ExistsAsync(id);
        }

        public async Task<Category?> GetDeletedCategoryByNameAsync(string name)
        {
            return await this.categoryRepository.GetDeletedCategoryByNameAsync(name);
        }

        public async Task<CategoryFormInputViewModel?> GetCategoryForRestoreByIdAsync(int id)
        {
            Category? category = await this.categoryRepository
                .GetAllAttached()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return null;
            }

            CategoryFormInputViewModel modelToRestore = MapToCategoryFormInputViewModel(category);

            return modelToRestore;
        }

        public async Task<bool> RestoreByIdAsync(int id)
        {
            Category? category = await this.categoryRepository
                .GetAllAttached()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted);

            if (category == null)
            {
                return false;
            }

            category.IsDeleted = false;

            this.categoryRepository.Update(category);
            await this.categoryRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CategoryManageViewModel>> GetAllAsync()
        {
            IEnumerable<CategoryManageViewModel> categories = await this.categoryRepository
                .GetAllAttached()
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryManageViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl ?? DefaultImageUrl,
                })
                .ToListAsync();

            return categories;
        }

        private static CategoryFormInputViewModel MapToCategoryFormInputViewModel(Category category)
        {
            return new CategoryFormInputViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl ?? DefaultImageUrl,
            };
        }

        private static DeleteCategoryViewModel MapToDeleteCategoryViewModel(Category category)
        {
            return new DeleteCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
            };
        }
    }
}