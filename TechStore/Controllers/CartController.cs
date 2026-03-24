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
        private Guid UserId => Guid.Parse(this.GetUserId()!);

        public CartController(ICartService cartService,
            ILogger<CartController> logger)
        {
            this.cartService = cartService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Guid userId = this.UserId;

            CartViewModel? model = await cartService.GetCartAsync(userId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string? productId, int quantity)
        {
            if (!TryParseProductId(productId, out Guid parsedProductId))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                Guid userId = this.UserId;
                bool result = await cartService.AddProductAsync(userId, parsedProductId, quantity);

                if (result == false)
                {
                    logger.LogWarning(CartLog.ProductAddFailed, parsedProductId, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation(CartLog.ProductAdded, parsedProductId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, CartLog.ProductAddError, parsedProductId);
                TempData[TempDataKeys.ErrorMessage] = CartUi.ProductAddError;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Increase(string? productId)
        {
            if (!TryParseProductId(productId, out Guid parsedProductId))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                Guid userId = this.UserId;
                bool result = await cartService.IncreaseProductQuantityAsync(userId, parsedProductId);

                if (result == false)
                {
                    logger.LogWarning(CartLog.ProductIncreaseFailed, parsedProductId, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation(CartLog.ProductIncreaseSuccess, parsedProductId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, CartLog.ProductIncreaseError, parsedProductId);
                TempData[TempDataKeys.ErrorMessage] = CartUi.IncreaseError;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(string? productId)
        {
            if (!TryParseProductId(productId, out Guid parsedProductId))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                Guid userId = this.UserId;
                bool result = await cartService.RemoveProductAsync(userId, parsedProductId);

                if (result == false)
                {
                    logger.LogWarning(CartLog.ProductRemoveFailed, parsedProductId, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation(CartLog.ProductRemoved, parsedProductId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, CartLog.ProductRemoveError, parsedProductId);
                TempData[TempDataKeys.ErrorMessage] = CartUi.RemoveError;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decrease(string? productId)
        {
            if (!TryParseProductId(productId, out Guid parsedProductId))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                Guid userId = this.UserId;
                bool result = await cartService.DecreaseProductAsync(userId, parsedProductId);

                if (result == false)
                {
                    logger.LogWarning(CartLog.ProductDecreaseFailed, parsedProductId, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation(CartLog.ProductDecreased, parsedProductId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, CartLog.ProductDecreaseError, parsedProductId);
                TempData[TempDataKeys.ErrorMessage] = CartUi.DecreaseError;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            Guid userId = this.UserId;

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

        private bool TryParseProductId(string? productId, out Guid parsedProductId)
        {
            return Guid.TryParse(productId, out parsedProductId);
        }
    }
}