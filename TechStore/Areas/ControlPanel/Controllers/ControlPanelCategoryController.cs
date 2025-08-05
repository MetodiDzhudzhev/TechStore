using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Category;
using static TechStore.GCommon.ValidationConstants.Shared;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    public class ControlPanelCategoryController : BaseControlPanelController
    {
        private readonly ICategoryService categoryService;

        private readonly ILogger<ControlPanelCategoryController> logger;

        private const int PageSize = 4;

        public ControlPanelCategoryController(ICategoryService categoryService,
            ILogger<ControlPanelCategoryController> logger)
        {
            this.categoryService = categoryService;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add(CategoryFormInputViewModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning("Attempt by user {UserId} to add category with invalid model state.", this.GetUserId());
                    return this.View(inputModel);
                }

                if (await categoryService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    logger.LogWarning("Attempt to add category name that already exists - {CategoryName}.", inputModel.Name);
                    var deletedCategory = await this.categoryService
                        .GetDeletedCategoryByNameAsync(inputModel.Name);

                    if (deletedCategory != null)
                    {
                        return RedirectToAction(nameof(Restore), new { id = deletedCategory.Id });
                    }

                    else
                    {
                        ModelState.AddModelError(nameof(inputModel.Name), "Category with this name already exists.");
                        return View(inputModel);
                    }
                }
                bool result = await this.categoryService.AddCategoryAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    logger.LogWarning("Failed to add category with name '{CategoryName}' by user {UserId}.", inputModel.Name, this.GetUserId());
                    ModelState.AddModelError(string.Empty, "Error occured while adding the category.");
                    return this.View(inputModel);
                }

                logger.LogInformation("Category '{CategoryName}' successfully added by user {UserId}!", inputModel.Name, this.GetUserId());
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while adding category '{CategoryName}'.", inputModel.Name);
                TempData["ErrorMessage"] = "An error occurred while adding the category. Please try again.";
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Restore(int id)
        {
            CategoryFormInputViewModel? category = await this.categoryService.GetCategoryForRestoreByIdAsync(id);

            if (category == null)
            {
                logger.LogWarning("Restore attempt for non-existing category with Id {CategoryId} by user {UserId}", id, this.GetUserId());
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            try
            {
                bool result = await this.categoryService.RestoreByIdAsync(id);

                if (result == false)
                {
                    logger.LogError("Failed to restore category with Id {CategoryId}.", id);
                    return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
                }

                logger.LogInformation("Category with Id {CategoryId} was successfully restored by user {UserId}.", id, this.GetUserId());
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while restoring category with Id {CategoryId}!", id);
                TempData["ErrorMessage"] = "We couldn't restore the category. Please try again later.";
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
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
                    logger.LogWarning("Category with Id {CategoryId} was not found", categoryId);
                    return NotFound();
                }

                return this.View(editableCategory);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while preparing Edit category form for category with Id {CategoryId}!", categoryId);
                TempData["ErrorMessage"] = "An error occurred while preparing the Edit category form.";
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    logger.LogWarning("Attempt to edit category with Id {CategoryId} with invalid model state by user {UserId}.", inputModel.Id, this.GetUserId());
                    ModelState.AddModelError(string.Empty, "Error occured while editing the category. Please review the details and try again.");
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
                    logger.LogWarning("Failed to edit category '{CategoryName}'!", inputModel.Name);
                    this.ModelState.AddModelError(String.Empty, "Error occured while editing the category!");
                    return View(inputModel);
                }

                logger.LogInformation("Category '{CategoryName}' successfully edited by user {UserId}.", inputModel.Name, this.GetUserId());
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while editing category '{CategoryName}'!", inputModel.Name);
                TempData["ErrorMessage"] = "An unexpected error occurred while editing the category. Please try again.";
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
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
                    logger.LogWarning("Category with Id {CategoryId} was not found!", id);
                    return NotFound();
                }

                return this.View(categoryToDelete);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception while preparing Delete category form for category with Id {CategoryId}!", id);
                TempData["ErrorMessage"] = "An error occurred while preparing the Delete category form.";
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(DeleteCategoryViewModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning("Attempt to delete category with Id {CategoryId} with invalid model state by user {UserId}", model.Id, this.GetUserId());
                    ModelState.AddModelError(string.Empty, "Please do not modify the page");
                    return this.View(model);
                }

                bool result = await this.categoryService
                    .SoftDeleteCategoryAsync(this.GetUserId()!, model);

                if (result == false)
                {
                    logger.LogWarning("Failed to delete category with Id {CategoryId} by user {UserId}!", model.Id, this.GetUserId());
                    this.ModelState.AddModelError(string.Empty, "Error occured while deleting the category!");
                    return this.View(model);
                }

                logger.LogInformation("Category with Id {CategoryId} successfully deleted by user {UserId}.", model.Id, this.GetUserId());
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while deleting category with Id {CategoryId}.", model.Id);
                TempData["ErrorMessage"] = "An error occurred while deleting the category. Please try again.";
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage(int page = 1)
        {
            int totalCount = await categoryService.GetTotalCountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (page > totalPages && totalPages > 0)
            {
                page = totalPages;
            }

            IEnumerable<CategoryManageViewModel> categories = await categoryService.GetPagedAsync(page, PageSize);

            CategoryManageListViewModel viewModel = new CategoryManageListViewModel
            {
                Categories = categories,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }
    }
}
