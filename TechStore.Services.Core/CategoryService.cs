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
        private readonly IUserRepository userRepository;

        public CategoryService(ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            this.categoryRepository = categoryRepository;
            this.userRepository = userRepository;
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
        public async Task<bool> AddCategoryAsync(string userId, CategoryFormInputViewModel inputModel)
        {
            bool result = false;

            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            if (user != null)
            {
                Category category = new Category()
                {
                    Name = inputModel.Name.Trim(),
                    ImageUrl = inputModel.ImageUrl ?? DefaultImageUrl,
                };

                await this.categoryRepository.AddAsync(category);
                result = true;
            }

            return result;
        }

        public async Task<CategoryFormInputViewModel?> GetEditableCategoryByIdAsync(string userId, int? categoryId)
        {

            if (categoryId == null || categoryId <= 0)
            {
                return null;
            }

            CategoryFormInputViewModel? editableCategory = null;

            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            if (user != null)
            {
                Category? category = await this.categoryRepository
                    .GetAllAttached()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(c => c.Id == categoryId);

                if (category == null)
                {
                    return null;
                }

                editableCategory = new CategoryFormInputViewModel()
                {
                    Id = category.Id,
                    Name = category!.Name,
                    ImageUrl = category.ImageUrl ?? DefaultImageUrl,
                };
            }
            return editableCategory;
        }

        public async Task<bool> EditCategoryAsync(string userId, CategoryFormInputViewModel inputModel)
        {
            bool result = false;

            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            if (user != null)
            {
                Category? editableCategory = await this.categoryRepository
                .GetByIdAsync(inputModel.Id);

                if (editableCategory != null)
                {
                    editableCategory.Id = inputModel.Id;
                    editableCategory.Name = inputModel.Name.Trim();
                    editableCategory.ImageUrl = inputModel.ImageUrl ?? DefaultImageUrl;
                    await this.categoryRepository.UpdateAsync(editableCategory);

                    result = true;
                }
            }

            return result;
        }

        public async Task<DeleteCategoryViewModel?> GetCategoryForDeleteByIdAsync(string userId, int? categoryId)
        {
            DeleteCategoryViewModel? categoryForDeleteViewModel = null;

            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            if (user != null && categoryId != null)
            {

                Category? category = await this.categoryRepository
                    .GetAllAttached()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(c => c.Id == categoryId);

                if (category != null && category.IsDeleted == false)
                {
                    categoryForDeleteViewModel = new DeleteCategoryViewModel()
                    {
                        Id = category.Id,
                        Name = category.Name,
                        ImageUrl = category.ImageUrl,
                    };
                }
            }

            return categoryForDeleteViewModel;
        }

        public async Task<bool> SoftDeleteCategoryAsync(string userId, DeleteCategoryViewModel deleteModel)
        {
            bool result = false;

            User? user = await this.userRepository
                .GetByIdAsync(Guid.Parse(userId));

            Category? categoryToDelete = await this.categoryRepository
                .GetByIdAsync(deleteModel.Id);

            if (user != null && categoryToDelete != null)
            {
                result = await this.categoryRepository.DeleteAsync(categoryToDelete);
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
                    Name = c.Name.Trim(),
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

            CategoryFormInputViewModel modelToRestore = new CategoryFormInputViewModel()
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl ?? DefaultImageUrl,
            };

            return modelToRestore;
        }

        public async Task<bool> RestoreByIdAsync(int id)
        {
            Category? category = await this.categoryRepository
                .GetAllAttached()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted);

            if (category == null)
            {
                return false;
            }

            category.IsDeleted = false;

            await this.categoryRepository.UpdateAsync(category);
            return true;
        }

    }
}
