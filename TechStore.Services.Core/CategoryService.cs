using Microsoft.EntityFrameworkCore;
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
    }
}
