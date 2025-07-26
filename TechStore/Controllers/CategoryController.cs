using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Category;
using static TechStore.GCommon.ValidationConstants.Shared;


namespace TechStore.Web.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<AllCategoriesIndexViewModel?> allCategories = await this.categoryService
                .GetAllCategoriesAsync();

            return View(allCategories);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add(CategoryFormInputViewModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(inputModel);
                }

                if (await categoryService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    ModelState.AddModelError(nameof(inputModel.Name), "Category with this name already exists.");
                    return View(inputModel);
                }
                bool result = await this.categoryService.AddCategoryAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    ModelState.AddModelError(string.Empty, "Error occured while adding a category");
                    return this.View(inputModel);
                }

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                //TODO: Implement it with the ILogger
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? categoryId)
        {
            try
            {
                CategoryFormInputViewModel? editableCategory = await this.categoryService
                    .GetEditableCategoryByIdAsync(this.GetUserId(), categoryId);

                if (editableCategory == null)
                {
                    // TODO: Custom 404 page
                    return this.RedirectToAction(nameof(Index));
                }

                return this.View(editableCategory);
            }
            catch (Exception e)
            {
                // TODO: Implement it with the ILogger
                // TODO: Add JS bars
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(CategoryFormInputViewModel inputModel)
        {
            try
            {
                if (inputModel.Id > IntIdMaxValue || inputModel.Id < IntIdMinValue)
                {
                    ModelState.AddModelError(nameof(inputModel.Id), "Invalid Id.");
                }

                if (!this.ModelState.IsValid)
                {
                    return this.View(inputModel);
                }

                if (await categoryService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    ModelState.AddModelError(nameof(inputModel.Name), "Category with this name already exists.");
                    return View(inputModel);
                }

                bool result = await this.categoryService.EditCategoryAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    // TODO: Custom 404 page
                    return this.RedirectToAction(nameof(Index));
                }

                return this.RedirectToAction(nameof(Index), new { id = inputModel.Id });
            }
            catch (Exception e)
            {
                // TODO: Implement it with the ILogger
                // TODO: Add JS bars
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                DeleteCategoryViewModel? categoryToDelete = await this.categoryService
                    .GetCategoryForDeleteByIdAsync(this.GetUserId(), id);

                if (categoryToDelete == null)
                {
                    //TODO: Custom 404 page
                    return this.RedirectToAction(nameof(Index));
                }

                return this.View(categoryToDelete);
            }
            catch (Exception e)
            {
                // TODO: Implement it with the ILogger
                // TODO: Add JS bars 
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(DeleteCategoryViewModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Please do not modify the page");
                    return this.RedirectToAction(nameof(Index));
                }

                bool result = await this.categoryService
                    .SoftDeleteCategoryAsync(this.GetUserId()!, model);

                if (result == false)
                {
                    this.ModelState.AddModelError(string.Empty, "Fatal error occured while deleting the category!");
                    // TODO: Implement JS or redirect to Not Found page

                    return this.View(model);
                }

                // TODO: Implement Js for success notification
                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                // TODO: Implement it with the ILogger
                // TODO: Add JS bars
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index));
            }
        }
    }
}
