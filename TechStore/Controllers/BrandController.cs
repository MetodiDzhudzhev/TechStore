using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Brand;

using TechStore.GCommon;
using BrandLog = TechStore.GCommon.LogMessages.Brand;
using BrandUi = TechStore.GCommon.UiMessages.Brand;

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
                    logger.LogWarning(BrandLog.NotFound, id);
                    return NotFound();
                }

                return this.View(brandDetails);
            }
            catch (Exception e)
            {
                logger.LogError(e, BrandLog.DetailsLoadError, id);
                TempData[TempDataKeys.ErrorMessage] = BrandUi.DetailsLoadError;
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }
    }
}
