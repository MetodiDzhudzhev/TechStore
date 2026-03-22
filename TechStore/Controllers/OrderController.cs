using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Order;

using TechStore.GCommon;
using OrderLog = TechStore.GCommon.LogMessages.Order;
using OrderUi = TechStore.GCommon.UiMessages.Order;

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
                logger.LogInformation(OrderLog.InvalidCheckoutForm, userId);
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
                    logger.LogWarning(OrderLog.CreateFailed, userId);
                    ModelState.AddModelError(string.Empty, OrderUi.CreateFailed);
                    
                    return View(model);
                }

                logger.LogInformation(OrderLog.Create, userId, orderId.Value);
                return this.RedirectToAction("Payment", "Payment", new { id = orderId.Value });
            }
            catch (Exception e)
            {
                logger.LogError(e, OrderLog.CheckoutError, userId);
                TempData[TempDataKeys.ErrorMessage] = OrderUi.CheckoutError;
                
                return this.RedirectToAction("Checkout");
            }
        }
    }
}
