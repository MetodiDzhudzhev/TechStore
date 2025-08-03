using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Brand;

namespace TechStore.Web.Controllers
{
    public class BrandController : BaseController
    {
        private readonly IBrandService brandService;

        private readonly ILogger<BrandController> logger;

        public BrandController(IBrandService brandService,
            ILogger<BrandController> logger)
        {
            this.brandService = brandService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                BrandDetailsViewModel? brandDetails = await this.brandService
                    .GetBrandDetailsViewModelAsync(id);

                if (brandDetails == null)
                {
                    logger.LogWarning("Brand with Id {BrandId} was not found", id);
                    return NotFound();
                }

                return this.View(brandDetails);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred while loading details for brand with Id {BrandId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the brand details. Please try again later.";
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }
    }
}
