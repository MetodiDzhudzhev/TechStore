using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.User;

using TechStore.GCommon;
using AccountLog = TechStore.GCommon.LogMessages.Account;
using AccountUi = TechStore.GCommon.UiMessages.Account;

namespace TechStore.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class AccountController : BaseAccountController
    {
        private readonly IOrderService orderService;
        private readonly IUserService userService;
        private readonly IReviewService reviewService;
        private readonly ILogger<AccountController> logger;

        private const int PageSize = 5;
        
        public AccountController(IOrderService orderService, 
            IUserService userService,
            IReviewService reviewService,
            ILogger<AccountController> logger)
        {
            this.orderService = orderService;
            this.userService = userService;
            this.reviewService = reviewService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders(int page = 1)
        {
            string? userId = this.GetUserId();

            if (!Guid.TryParse(userId, out Guid currentUserId))
            {
                return Unauthorized();
            }

            var viewModel = await orderService.GetMyOrdersPagedAsync(currentUserId, page, PageSize);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DeliveryDetails()
        {
            string? userId = this.GetUserId();

            if (!Guid.TryParse(userId, out Guid currentUserId))
            {
                logger.LogWarning(AccountLog.InvalidUserId);
                return Unauthorized();
            }

            try
            {
                DeliveryDetailsViewModel model = await userService.GetDeliveryDetailsAsync(currentUserId);
                return View(model);
            }
            catch (InvalidOperationException e)
            {
                logger.LogWarning(e, AccountLog.DeliveryDetailsUserNotFound, currentUserId);
                return Unauthorized();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeliveryDetails(DeliveryDetailsViewModel model)
        {
            string? userId = this.GetUserId();

            if (!Guid.TryParse(userId, out Guid currentUserId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool updated = await userService.UpdateDeliveryDetailsAsync(currentUserId, model);

            if (!updated)
            {
                ModelState.AddModelError(string.Empty, AccountUi.DeliveryDetailsUpdateError);
                logger.LogWarning(AccountLog.DeliveryDetailsUpdateFailed, currentUserId);
                return View(model);
            }

            TempData[TempDataKeys.SuccessMessage] = AccountUi.DeliveryDetailsUpdateSuccess;
            logger.LogInformation(AccountLog.DeliveryDetailsUpdateSuccess, currentUserId);
            return RedirectToAction(nameof(DeliveryDetails));
        }

        [HttpGet]
        public async Task<IActionResult> MyReviews(int page = 1)
        {
            string? userId = this.GetUserId();

            if (!Guid.TryParse(userId, out Guid currentUserId))
            {
                return Unauthorized();
            }

            var viewModel = await reviewService.GetMyReviewsPagedAsync(currentUserId, page, PageSize);
            return View(viewModel);
        }
    }
}
