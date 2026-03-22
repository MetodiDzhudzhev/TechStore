using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;

using TechStore.GCommon;
using ReviewLog = TechStore.GCommon.LogMessages.Review;
using ReviewUi = TechStore.GCommon.UiMessages.Review;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    public class ControlPanelReviewController : BaseControlPanelController
    {

        private readonly IReviewService reviewService;
        private readonly ILogger<ControlPanelReviewController> logger;

        private const int PageSize = 5;

        public ControlPanelReviewController(IReviewService reviewService,
            ILogger<ControlPanelReviewController> logger)
        {
            this.reviewService = reviewService;
            this.logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                bool deleted = await reviewService.SoftDeleteReviewAsync(id);

                if (!deleted)
                {
                    logger.LogWarning(ReviewLog.DeleteFailed, id);
                    TempData[TempDataKeys.ErrorMessage] = string.Format(ReviewUi.DeleteFailed, id);
                }

                logger.LogInformation(ReviewLog.DeleteSuccess, id);
                TempData[TempDataKeys.SuccessMessage] = string.Format(ReviewUi.DeleteSuccess, id);
                return RedirectToAction(nameof(Manage));
            }
            catch(Exception e)
            {
                logger.LogError(e, ReviewLog.DeleteError, id);
                TempData[TempDataKeys.ErrorMessage] = string.Format(ReviewUi.DeleteError, id);
                return RedirectToAction(nameof(Manage));
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage(int page = 1)
        {
            string? userId = this.GetUserId();

            if (!Guid.TryParse(userId, out Guid currentUserId))
            {
                logger.LogWarning(ReviewLog.InvalidUserId);
                return Unauthorized();
            }

            var viewModel = await reviewService.GetManageReviewsPageAsync(page, PageSize);
            return View(viewModel);
        }
    }
}