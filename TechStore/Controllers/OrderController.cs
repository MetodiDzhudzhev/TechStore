using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Order;

namespace TechStore.Web.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;
        private readonly ILogger<OrderController> logger;

        public OrderController(IOrderService orderService,
            ILogger<OrderController> logger)
        {
            this.orderService = orderService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            string? userId = this.GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            OrderDeliveryDetailsViewModel? model = await orderService.GetCheckoutDeliveryDetailsAsync(userId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderDeliveryDetailsViewModel model)
        {
            string? userId = this.GetUserId();

            if (userId == null)
            {
                return this.RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                logger.LogInformation("Checkout form submitted by user {UserId} is invalid.", userId);
                return View(model);
            }

            try
            {
                CreateOrderViewModel createOrderModel = new CreateOrderViewModel
                {
                    RecipientName = model.RecipientName,
                    ShippingAddress = model.ShippingAddress,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email
                };

                long? orderId = await orderService.CreateOrderAsync(userId, createOrderModel);

                if (orderId == null)
                {
                    logger.LogWarning("User {UserId} failed to create an order.", userId);
                    ModelState.AddModelError(string.Empty, "Failed to create order. Please check product availability.");
                    
                    return View(model);
                }

                logger.LogInformation("User {UserId} successfully created order {OrderId}.", userId, orderId.Value);
                return this.RedirectToAction("Payment", "Payment", new { id = orderId.Value });
            }
            catch (Exception e)
            {
                logger.LogError(e, "An unexpected error occurred while user {UserId} tried to checkout.", userId);
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
                
                return this.RedirectToAction("Checkout");
            }
        }
    }
}
