using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Category;
using static TechStore.GCommon.ValidationConstants.Shared;

using TechStore.GCommon;
using CategoryLog = TechStore.GCommon.LogMessages.Category;
using CategoryUi = TechStore.GCommon.UiMessages.Category;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    public class ControlPanelCategoryController : BaseControlPanelController
    {
        private readonly ICategoryService categoryService;

        private readonly ILogger<ControlPanelCategoryController> logger;

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
                    logger.LogWarning(CategoryLog.AddWithInvalidModelState, this.GetUserId());
                    return this.View(inputModel);
                }

                if (await categoryService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    logger.LogWarning(CategoryLog.NameAlreadyExist, inputModel.Name);
                    var deletedCategory = await this.categoryService
                        .GetDeletedCategoryByNameAsync(inputModel.Name);

                    if (deletedCategory != null)
                    {
                        return RedirectToAction(nameof(Restore), new { id = deletedCategory.Id });
                    }

                    else
                    {
                        ModelState.AddModelError(nameof(inputModel.Name), CategoryUi.NameAlreadyExist);
                        return View(inputModel);
                    }
                }
                bool result = await this.categoryService.AddCategoryAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    logger.LogWarning(CategoryLog.AddFailed, inputModel.Name, this.GetUserId());
                    ModelState.AddModelError(string.Empty, CategoryUi.AddError);
                    return this.View(inputModel);
                }

                logger.LogInformation(CategoryLog.AddSuccess, inputModel.Name, this.GetUserId());
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
            catch (Exception e)
            {
                logger.LogError(e, CategoryLog.AddError, inputModel.Name);
                TempData[TempDataKeys.ErrorMessage] = CategoryUi.AddError;
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
                logger.LogWarning(CategoryLog.RestoreInvalidCategory, id, this.GetUserId());
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
                    logger.LogWarning(CategoryLog.RestoreFailed, id);
                    TempData[TempDataKeys.ErrorMessage] = CategoryUi.RestoreFailed;
                    return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
                }

                logger.LogInformation(CategoryLog.RestoreSuccess, id, this.GetUserId());
                TempData[TempDataKeys.SuccessMessage] = CategoryUi.RestoreSuccess;
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
            catch (Exception e)
            {
                logger.LogError(e, CategoryLog.RestoreError, id);
                TempData[TempDataKeys.ErrorMessage] = CategoryUi.RestoreError;
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
                    logger.LogWarning(CategoryLog.NotFound, categoryId);
                    return NotFound();
                }

                return this.View(editableCategory);
            }
            catch (Exception e)
            {
                logger.LogError(e, CategoryLog.EditCategoryPageLoadError, categoryId);
                TempData[TempDataKeys.ErrorMessage] = CategoryUi.EditCategoryPageLoadError;
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
                    ModelState.AddModelError(nameof(inputModel.Id), CategoryUi.InvalidId);
                }

                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning(CategoryLog.EditWithInvalidModelState, inputModel.Id, this.GetUserId());
                    ModelState.AddModelError(string.Empty, CategoryUi.EditWithInvalidModelState);
                    return this.View(inputModel);
                }

                if (await categoryService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    logger.LogWarning(CategoryLog.NameAlreadyExist, inputModel.Name);
                    ModelState.AddModelError(nameof(inputModel.Name), CategoryUi.NameAlreadyExist);
                    return View(inputModel);
                }

                bool result = await this.categoryService.EditCategoryAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    logger.LogWarning(CategoryLog.EditFailed, inputModel.Name);
                    return View(inputModel);
                }

                logger.LogInformation(CategoryLog.EditSuccess, inputModel.Name, this.GetUserId());
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
            catch (Exception e)
            {
                logger.LogError(e, CategoryLog.EditError, inputModel.Name);
                TempData[TempDataKeys.ErrorMessage] = CategoryUi.EditError;
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
                    logger.LogWarning(CategoryLog.NotFound, id);
                    return NotFound();
                }

                return this.View(categoryToDelete);
            }
            catch (Exception e)
            {
                logger.LogError(e, CategoryLog.DeleteCategoryPageLoadError, id);
                TempData[TempDataKeys.ErrorMessage] = CategoryUi.DeleteCategoryPageLoadError;
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
                    logger.LogWarning(CategoryLog.DeleteWithInvalidModelState, model.Id, this.GetUserId());
                    return this.View(model);
                }

                bool result = await this.categoryService
                    .SoftDeleteCategoryAsync(this.GetUserId()!, model);

                if (result == false)
                {
                    logger.LogWarning(CategoryLog.DeleteFailed, model.Id, this.GetUserId());
                    this.ModelState.AddModelError(string.Empty, CategoryUi.DeleteFailed);
                    return this.View(model);
                }

                logger.LogInformation(CategoryLog.DeleteSuccess, model.Id, this.GetUserId());
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
            catch (Exception e)
            {
                logger.LogError(e, CategoryLog.DeleteError, model.Id);
                TempData[TempDataKeys.ErrorMessage] = CategoryUi.DeleteError;
                return this.RedirectToAction(nameof(Index), "Category", new { area = "" });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage()
        {
            IEnumerable<CategoryManageViewModel> categories = await categoryService.GetAllAsync();

            CategoryManageListViewModel viewModel = new CategoryManageListViewModel
            {
                Categories = categories
            };

            return View(viewModel);
        }
    }
}
