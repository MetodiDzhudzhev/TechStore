using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Category;
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
                .Select(c => new AllCategoriesIndexViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl,
                })
                .ToListAsync();

            foreach (AllCategoriesIndexViewModel category in allCategories)
            {
                if (String.IsNullOrEmpty(category.ImageUrl))
                {
                    category.ImageUrl = DefaultImageUrl;
                }
            }

            return allCategories;
        }
        public async Task AddCategoryAsync(CategoryFormInputViewModel inputModel)
        {
            Category category = new Category()
            {
                Name = inputModel.Name,
                ImageUrl = inputModel.ImageUrl,
            };

            await this.categoryRepository.AddAsync(category);
        }

        public async Task<CategoryFormEditViewModel?> GetEditableCategoryByIdAsync(int? id)
        {
            CategoryFormEditViewModel? editableCategory = await this.categoryRepository
                .GetAllAttached()
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryFormEditViewModel()
                {
                    Name = c.Name,
                    ImageUrl = c.ImageUrl,
                })
                .SingleOrDefaultAsync();

            return editableCategory;
        }

        public async Task<bool> EditCategoryAsync(CategoryFormEditViewModel inputModel)
        {
            bool result = false;

            Category? editableCategory = await this.categoryRepository.GetByIdAsync(inputModel.Id);
            if (editableCategory == null)
            {
                return false;
            }

            editableCategory.Name = inputModel.Name;
            editableCategory.ImageUrl = inputModel.ImageUrl;

            result = await this.categoryRepository.UpdateAsync(editableCategory);

            return result;
        }
    }
}
