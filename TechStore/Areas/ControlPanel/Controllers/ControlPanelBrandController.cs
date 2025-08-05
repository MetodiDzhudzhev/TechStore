using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Brand;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    public class ControlPanelBrandController : BaseControlPanelController
    {
        private readonly IBrandService brandService;

        private readonly ILogger<ControlPanelBrandController> logger;

        private const int PageSize = 4;

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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add(BrandFormInputViewModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning("Attempt by user {UserId} to add brand with invalid model state.", this.GetUserId());
                    return this.View(inputModel);
                }

                if (await brandService.ExistsByNameAsync(inputModel.Name, inputModel.Id))
                {
                    logger.LogWarning("Attempt to add brand name that already exists - {BrandName}.", inputModel.Name);
                    var deletedBrand = await this.brandService
                        .GetDeletedBrandByNameAsync(inputModel.Name);

                    if (deletedBrand != null)
                    {
                        return RedirectToAction(nameof(Restore), new { id = deletedBrand.Id });
                    }

                    else
                    {
                        ModelState.AddModelError(nameof(inputModel.Name), "Brand with this name already exists.");
                        return View(inputModel);
                    }
                }
                bool result = await this.brandService.AddBrandAsync(this.GetUserId()!, inputModel);

                if (result == false)
                {
                    logger.LogWarning("Failed to add brand with name '{BrandName}' by user {UserId}.", inputModel.Name, this.GetUserId());
                    ModelState.AddModelError(string.Empty, "Error occured while adding the brand.");
                    return this.View(inputModel);
                }

                logger.LogInformation("Brand '{BrandName}' successfully added by user {UserId}!", inputModel.Name, this.GetUserId());
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while adding brand '{BrandName}'.", inputModel.Name);
                TempData["ErrorMessage"] = "An error occurred while adding the brand. Please try again.";
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
                logger.LogWarning("Restore attempt for non-existing brand with Id {BrandId} by user {UserId}", id, this.GetUserId());
                return NotFound();
            }

            return View(brand);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            try
            {
                bool result = await this.brandService.RestoreByIdAsync(id);

                if (result == false)
                {
                    logger.LogError("Failed to restore brand with Id {BrandId}.", id);
                    return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
                }

                logger.LogInformation("Brand with Id {BrandId} was successfully restored by user {UserId}.", id, this.GetUserId());
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while restoring brand with Id {BrandId}!", id);
                TempData["ErrorMessage"] = "We couldn't restore the brand. Please try again later.";
                return this.RedirectToAction(nameof(Manage), "ControlPanelBrand");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage(int page = 1)
        {
            int totalCount = await brandService.GetTotalCountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (page > totalPages && totalPages > 0)
            {
                page = totalPages;
            }

            IEnumerable<BrandManageViewModel> brands = await brandService.GetPagedAsync(page, PageSize);

            BrandManageListViewModel viewModel = new BrandManageListViewModel
            {
                Brands = brands,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }
    }
}
