using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Product;

namespace TechStore.Web.ViewComponents
{
    public class CategoryDropdownViewComponent : ViewComponent
    {
        private readonly ICategoryService categoryService;

        public CategoryDropdownViewComponent(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<AddProductCategoryDropDownModel> categories =
                await this.categoryService.GetCategoriesDropDownDataAsync();

            return View(categories);
        }
    }
}
