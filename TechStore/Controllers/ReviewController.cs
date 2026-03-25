using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Review;

using TechStore.GCommon;
using ReviewLog = TechStore.GCommon.LogMessages.Review;
using ReviewUi = TechStore.GCommon.UiMessages.Review;

namespace TechStore.Web.Controllers
{
    public class ReviewController : BaseController
    {
        private readonly IReviewService reviewService;
        private readonly ILogger<ReviewController> logger;

        private Guid UserId => Guid.Parse(this.GetUserId()!);

        public ReviewController(IReviewService reviewService,
            ILogger<ReviewController> logger)
        {
            this.reviewService = reviewService;
            this.logger = logger;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ReviewFormInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning(ReviewLog.InvalidModelState, this.UserId);
                    TempData[TempDataKeys.ErrorMessage] = ReviewUi.InvalidInput;
                    return Redirect($"/Product/Details/{inputModel.ProductId}#reviews");
                }

                bool result = await reviewService.Add(this.UserId, inputModel);

                if (result == false)
                {
                    logger.LogWarning(ReviewLog.AddFailed, this.UserId, inputModel.ProductId);
                    TempData[TempDataKeys.ErrorMessage] = ReviewUi.AlreadyAdded;
                    return Redirect($"/Product/Details/{inputModel.ProductId}#reviews");
                }

                logger.LogInformation(ReviewLog.AddSuccess, this.UserId, inputModel.ProductId);
                TempData[TempDataKeys.SuccessMessage] = ReviewUi.AddSuccess;
                return Redirect($"/Product/Details/{inputModel.ProductId}#reviews");
            }
            catch (Exception e)
            {
                logger.LogError(e, ReviewLog.AddError, inputModel.ProductId);
                TempData[TempDataKeys.ErrorMessage] = ReviewUi.AddError;
                return Redirect($"/Product/Details/{inputModel.ProductId}#reviews");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Panel(Guid productId, int page = 1, int pageSize = 5)
        {
            Guid? userId = null;
            bool isAuthenticated = this.IsUserAuthenticated();

            if (isAuthenticated)
            {
                userId = this.UserId;
            }

            ReviewsPanelViewModel viewModel = await reviewService.GetPanelAsync(productId, userId, page, pageSize);
            return PartialView("ReviewsPanel", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Stats(Guid productId)
        {
            ReviewsStatsViewModel viewModel = await reviewService.GetStatsAsync(productId);
            return Json(viewModel);
        }
    }
}