using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    public class ControlPanelReviewController : BaseControlPanelController
    {

        private readonly IReviewService reviewService;

        private const int PageSize = 5;

        public ControlPanelReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            bool deleted = await reviewService.SoftDeleteReviewAsync(id);

            if (!deleted)
            {
                TempData["ErrorMessage"] = $"Review #{id} not found or already deleted.";
            }

            TempData["SuccessMessage"] = $"Review #{id} was successfully deleted.";
            return RedirectToAction(nameof(Manage));
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage(int page = 1)
        {
            string? userId = this.GetUserId();

            if (!Guid.TryParse(userId, out Guid currentUserId))
            {
                return Unauthorized();
            }

            var viewModel = await reviewService.GetManageReviewsPageAsync(page, PageSize);
            return View(viewModel);
        }
    }
}
