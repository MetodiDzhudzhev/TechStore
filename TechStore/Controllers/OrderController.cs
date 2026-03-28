using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Order;

using TechStore.GCommon;
using OrderLog = TechStore.GCommon.LogMessages.Order;
using OrderUi = TechStore.GCommon.UiMessages.Order;
using Microsoft.AspNetCore.Authorization;

namespace TechStore.Web.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;
        private readonly ILogger<OrderController> logger;

        private Guid UserId => Guid.Parse(this.GetUserId()!);

        public OrderController(IOrderService orderService,
            ILogger<OrderController> logger)
        {
            this.orderService = orderService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            OrderDeliveryDetailsViewModel? model = await orderService.GetCheckoutDeliveryDetailsAsync(this.UserId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderDeliveryDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                logger.LogInformation(OrderLog.InvalidCheckoutForm, this.UserId);
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

                long? orderId = await orderService.CreateOrderAsync(this.UserId, createOrderModel);

                if (orderId == null)
                {
                    logger.LogWarning(OrderLog.CreateFailed, this.UserId);
                    ModelState.AddModelError(string.Empty, OrderUi.CreateFailed);
                    
                    return View(model);
                }

                logger.LogInformation(OrderLog.Create, this.UserId, orderId.Value);
                return this.RedirectToAction("Payment", "Payment", new { id = orderId.Value });
            }
            catch (Exception e)
            {
                logger.LogError(e, OrderLog.CheckoutError, this.UserId);
                TempData[TempDataKeys.ErrorMessage] = OrderUi.CheckoutError;
                
                return this.RedirectToAction("Checkout");
            }
        }
    }
}