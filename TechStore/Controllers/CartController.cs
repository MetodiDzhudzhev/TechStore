using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Cart;

using TechStore.GCommon;
using CartLog = TechStore.GCommon.LogMessages.Cart;
using CartUi = TechStore.GCommon.UiMessages.Cart;

namespace TechStore.Web.Controllers
{
    [Authorize]
    public class CartController : BaseController
    {
        private readonly ICartService cartService;

        private readonly ILogger<CartController> logger;

        public CartController(ICartService cartService,
            ILogger<CartController> logger)
        {
            this.cartService = cartService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = this.GetUserId()!;

            CartViewModel? model = await cartService.GetCartAsync(userId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string? productId, int quantity)
        {
            try
            {
                string userId = this.GetUserId()!;
                bool result = await cartService.AddProductAsync(userId, productId, quantity);

                if (result == false)
                {
                    logger.LogWarning(CartLog.ProductAddFailed, productId, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation(CartLog.ProductAdded, productId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, CartLog.ProductAddError, productId);
                TempData[TempDataKeys.ErrorMessage] = CartUi.ProductAddError;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Increase(string? productId)
        {
            try
            {
                string userId = this.GetUserId()!;
                bool result = await cartService.IncreaseProductQuantityAsync(userId, productId);

                if (result == false)
                {
                    logger.LogWarning(CartLog.ProductIncreaseFailed, productId, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation(CartLog.ProductIncreaseSuccess, productId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, CartLog.ProductIncreaseError, productId);
                TempData[TempDataKeys.ErrorMessage] = CartUi.IncreaseError;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(string? productId)
        {
            try
            {
                string userId = this.GetUserId()!;
                bool result = await cartService.RemoveProductAsync(userId, productId);

                if (result == false)
                {
                    logger.LogWarning(CartLog.ProductRemoveFailed, productId, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation(CartLog.ProductRemoved, productId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, CartLog.ProductRemoveError, productId);
                TempData[TempDataKeys.ErrorMessage] = CartUi.RemoveError;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Decrease(string? productId)
        {
            try
            {
                string userId = this.GetUserId()!;
                bool result = await cartService.DecreaseProductAsync(userId, productId);

                if (result == false)
                {
                    logger.LogWarning(CartLog.ProductDecreaseFailed, productId, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation(CartLog.ProductDecreased, productId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, CartLog.ProductDecreaseError, productId);
                TempData[TempDataKeys.ErrorMessage] = CartUi.DecreaseError;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            string userId = this.GetUserId()!;

            try
            {
                bool result = await cartService.ClearCartAsync(userId);

                if (result == false)
                {
                    logger.LogWarning(CartLog.CartClearFailed, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation(CartLog.CartCleared, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, CartLog.CartClearError, userId);
                TempData[TempDataKeys.ErrorMessage] = CartUi.ClearError;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}