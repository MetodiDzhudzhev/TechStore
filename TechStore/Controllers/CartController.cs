using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Cart;

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

            CartViewModel model = await cartService.GetCartAsync(userId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string? productId)
        {
            try
            {
                string userId = this.GetUserId()!;
                bool result = await cartService.AddProductAsync(userId, productId);

                if (result == false)
                {
                    logger.LogWarning("Failed to add product with id {ProductId} to cart of user with id {UserId}!", productId, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation("Product with id {ProductId} successfully added to the cart of user with id {UserId}!", productId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while adding product {ProductId}.", productId);
                TempData["ErrorMessage"] = "An error occurred while adding the product. Please try again.";
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
                    logger.LogWarning("Failed to increase the quantity of product with id {ProductId} in the cart of user with id {UserId}!", productId, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation("Quantity of Product with id {ProductId} in the cart of user with id {UserId} successfully increased by one!", productId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while increasing the quantity of product with id {ProductId}.", productId);
                TempData["ErrorMessage"] = "An error occurred while increasing the quantity of the product. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
