using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Review;

namespace TechStore.Web.Controllers
{
    public class ReviewController : BaseController
    {
        private readonly IReviewService reviewService;
        private readonly ILogger<ReviewController> logger;

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
                    logger.LogWarning("Attempt by user {UserId} to add review with invalid model state.", this.GetUserId());
                    TempData["ErrorMessage"] = "Please select a rating (1-5) and try again.";
                    return Redirect($"/Product/Details/{inputModel.ProductId}#reviews");
                }

                bool result = await reviewService.Add(GetUserId()!, inputModel);

                if (result == false)
                {
                    logger.LogWarning("User {UserId} failed to added review to product {ProductId}", this.GetUserId(), inputModel.ProductId);
                    TempData["ErrorMessage"] = "You already have a review for this product.";
                    return Redirect($"/Product/Details/{inputModel.ProductId}#reviews");
                }

                logger.LogInformation("User {UserId} sucessfully added review to product {ProductId}", this.GetUserId(), inputModel.ProductId);
                TempData["SuccessMessage"] = "Your review was added successfully!";
                return Redirect($"/Product/Details/{inputModel.ProductId}#reviews");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while adding review to product '{ProductId}'.", inputModel.ProductId);
                TempData["ErrorMessage"] = "An error occurred while adding review. Please try again.";
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
                userId = Guid.Parse(GetUserId()!);
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
