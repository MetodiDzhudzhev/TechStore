using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Payment;

namespace TechStore.Web.Controllers
{
    [Authorize]
    public class PaymentController : BaseController
    {
        private readonly IOrderService orderService;
        private readonly ILogger<PaymentController> logger;

        public PaymentController(IOrderService orderService,
            ILogger<PaymentController> logger)
        {
            this.orderService = orderService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Payment(long id)
        {
            try
            {
                string? userId = this.GetUserId();

                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                PaymentSummaryViewModel? model = await orderService.GetPaymentSummaryAsync(userId, id);

                if (model == null)
                {
                    return NotFound();
                }

                return View(model);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unexpected error while loading payment page. OrderId: {OrderId}", id);
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCheckoutSession(long orderId)
        {
            string? userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            PaymentSummaryViewModel? summary =
                await orderService.GetPaymentSummaryAsync(userId, orderId);

            if (summary == null)
            {
                return NotFound();
            }

            var domain = $"{Request.Scheme}://{Request.Host}";

            var products = await orderService.GetOrderProductsAsync(userId, orderId);

            if (products == null || products.Count == 0)
            {
                return NotFound();
            }

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = $"{domain}/Payment/Success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{domain}/Payment/Cancel?orderId={orderId}",
                PaymentMethodTypes = new List<string> { "card" },
                CustomerEmail = summary.Email,
                LineItems = new List<SessionLineItemOptions>()
            };

            foreach (var item in products)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    Quantity = item.Quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "eur",
                        UnitAmount = (long)(item.UnitPrice * 100m),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Images = string.IsNullOrWhiteSpace(item.Product.ImageUrl)
                                ? null
                                : new List<string> { item.Product.ImageUrl }
                        }
                    }
                });
            }

            options.ClientReferenceId = orderId.ToString();
            options.Metadata = new Dictionary<string, string>
            {
                ["orderId"] = orderId.ToString()
            };

            var service = new SessionService();
            Session session = service.Create(options);

            await orderService.AttachStripeSessionAsync(orderId, session.Id);

            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> Success(string session_id)
        {
            string? userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(session_id);

            var orderIdStr = session?.Metadata?["orderId"];
            if (!long.TryParse(orderIdStr, out var orderId))
            {
                return BadRequest();
            }    

            var model = await orderService.GetPaymentSuccessAsync(userId, orderId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Cancel(long orderId)
        {
            var model = new PaymentCancelViewModel 
            { 
                OrderId = orderId 
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCashOnDelivery(long orderId)
        {
            string? userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            PaymentSummaryViewModel? summary = await orderService.GetPaymentSummaryAsync(userId, orderId);
            if (summary == null)
            {
                return NotFound();
            }

            await orderService.MarkOrderAsCashOnDeliveryAsync(userId, orderId);
            return RedirectToAction(nameof(SuccessCashOnDelivery), new { orderId });
        }

        [HttpGet]
        public IActionResult SuccessCashOnDelivery(long orderId)
        {
            return View(orderId);
        }

    }
}
