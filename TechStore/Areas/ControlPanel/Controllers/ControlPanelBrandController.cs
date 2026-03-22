using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Brand;
using static TechStore.GCommon.ValidationConstants.Shared;

using TechStore.GCommon;
using BrandLog = TechStore.GCommon.LogMessages.Brand;
using BrandUi = TechStore.GCommon.UiMessages.Brand;


namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    public class ControlPanelBrandController : BaseControlPanelController
    {
        private readonly IBrandService brandService;

        private readonly ILogger<ControlPanelBrandController> logger;

        public ControlPanelBrandController(IBrandService brandService,
            ILogger<ControlPanelBrandController> logger)
        {
            this.brandService = brandService;
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
        public async Task<IActionResult> Add(BrandFormInputViewModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning(BrandLog.AddWithInvalidModelState, this.GetUserId());
                    return this.View(inputModel);
                }

                if (await brandService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    logger.LogWarning(BrandLog.NameAlreadyExist, inputModel.Name);
                    var deletedBrand = await this.brandService
                        .GetDeletedBrandByNameAsync(inputModel.Name);

                    if (deletedBrand != null)
                    {
                        return RedirectToAction(nameof(Restore), new { id = deletedBrand.Id });
                    }

                    else
                    {
                        ModelState.AddModelError(nameof(inputModel.Name), BrandUi.NameAlreadyExist);
                        return View(inputModel);
                    }
                }
                bool result = await this.brandService.AddBrandAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    logger.LogWarning(BrandLog.AddFailed, inputModel.Name, this.GetUserId());
                    ModelState.AddModelError(string.Empty, BrandUi.AddError);
                    return this.View(inputModel);
                }

                logger.LogInformation(BrandLog.AddSuccess, inputModel.Name, this.GetUserId());
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
            catch (Exception e)
            {
                logger.LogError(e, BrandLog.AddError, inputModel.Name);
                TempData[TempDataKeys.ErrorMessage] = BrandUi.AddError;
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Restore(int id)
        {
            BrandFormInputViewModel? brand = await this.brandService.GetBrandForRestoreByIdAsync(id);

            if (brand == null)
            {
                logger.LogWarning(BrandLog.RestoreInvalidBrand, id, this.GetUserId());
                return NotFound();
            }

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            try
            {
                bool result = await this.brandService.RestoreByIdAsync(id);

                if (result == false)
                {
                    logger.LogWarning(BrandLog.RestoreFailed, id);
                    TempData[TempDataKeys.ErrorMessage] = BrandUi.RestoreFailed;
                    return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
                }

                logger.LogInformation(BrandLog.RestoreSuccess, id, this.GetUserId());
                TempData[TempDataKeys.SuccessMessage] = BrandUi.RestoreSuccess;
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
            catch (Exception e)
            {
                logger.LogError(e, BrandLog.RestoreError, id);
                TempData[TempDataKeys.ErrorMessage] = BrandUi.RestoreError;
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? brandId)
        {
            try
            {
                BrandFormInputViewModel? editableBrand = await this.brandService
                    .GetEditableBrandByIdAsync(this.GetUserId(), brandId);

                if (editableBrand == null)
                {
                    logger.LogWarning(BrandLog.NotFound, brandId);
                    return NotFound();
                }

                return this.View(editableBrand);
            }
            catch (Exception e)
            {
                logger.LogError(e, BrandLog.EditBrandPageLoadError, brandId);
                TempData[TempDataKeys.ErrorMessage] = BrandUi.EditBrandPageLoadError;
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(BrandFormInputViewModel inputModel)
        {
            try
            {
                if (inputModel.Id > IntIdMaxValue || inputModel.Id < IntIdMinValue)
                {
                    ModelState.AddModelError(nameof(inputModel.Id), BrandUi.InvalidId);
                }

                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning(BrandLog.EditWithInvalidModelState, inputModel.Id, this.GetUserId());
                    ModelState.AddModelError(string.Empty, BrandUi.EditWithInvalidModelState);
                    return this.View(inputModel);
                }

                if (await brandService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    logger.LogWarning(BrandLog.NameAlreadyExist, inputModel.Name);
                    ModelState.AddModelError(nameof(inputModel.Name), BrandUi.NameAlreadyExist);
                    return View(inputModel);
                }

                bool result = await this.brandService.EditBrandAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    logger.LogWarning(BrandLog.EditFailed, inputModel.Name);
                    return View(inputModel);
                }

                logger.LogInformation(BrandLog.EditSuccess, inputModel.Name, this.GetUserId());
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
            catch (Exception e)
            {
                logger.LogError(e, BrandLog.EditError, inputModel.Name);
                TempData[TempDataKeys.ErrorMessage] = BrandUi.EditError;
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                DeleteBrandViewModel? brandToDelete = await this.brandService
                    .GetBrandForDeleteByIdAsync(this.GetUserId(), id);

                if (brandToDelete == null)
                {
                    logger.LogWarning(BrandLog.NotFound, id);
                    return NotFound();
                }

                return this.View(brandToDelete);
            }
            catch (Exception e)
            {
                logger.LogError(e, BrandLog.DeleteBrandPageLoadError, id);
                TempData[TempDataKeys.ErrorMessage] = BrandUi.DeleteBrandPageLoadError;
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(DeleteBrandViewModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning(BrandLog.DeleteWithInvalidModelState, model.Id, this.GetUserId());
                    return this.View(model);
                }

                bool result = await this.brandService
                    .SoftDeleteBrandAsync(this.GetUserId()!, model);

                if (result == false)
                {
                    logger.LogWarning(BrandLog.DeleteFailed, model.Id, this.GetUserId());
                    this.ModelState.AddModelError(string.Empty, BrandUi.DeleteFailed);
                    return this.View(model);
                }

                logger.LogInformation(BrandLog.DeleteSuccess, model.Id, this.GetUserId());
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
            catch (Exception e)
            {
                logger.LogError(e, BrandLog.DeleteError, model.Id);
                TempData[TempDataKeys.ErrorMessage] = BrandUi.DeleteError;
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage()
        {
            IEnumerable<BrandManageViewModel> brands = await brandService.GetAllAsync();

            BrandManageListViewModel viewModel = new BrandManageListViewModel
            {
                Brands = brands
            };

            return View(viewModel);
        }
    }
}