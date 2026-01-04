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
        public async Task<IActionResult> Add(string? productId, int quantity = 1)
        {
            try
            {
                string userId = this.GetUserId()!;
                bool result = await cartService.AddProductAsync(userId, productId, quantity);

                if (result == false)
                {
                    logger.LogWarning("Failed to add product {ProductId} to cart for user {UserId}!", productId, userId);
                    return RedirectToAction(nameof(Index));
                }

                logger.LogInformation("Product with id'{ProductId}' successfully added to the cart for user {UserId}!", productId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while adding product '{ProductId}'.", productId);
                TempData["ErrorMessage"] = "An error occurred while adding the product. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
