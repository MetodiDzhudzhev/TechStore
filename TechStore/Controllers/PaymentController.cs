using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Payment;

using TechStore.GCommon;
using static TechStore.GCommon.LogMessages.Payment;
using static TechStore.GCommon.UiMessages.Payment;

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
                    logger.LogWarning(UnauthenticatedPaymentAccess);
                    return RedirectToAction("Login", "Account");
                }

                PaymentSummaryViewModel? model = await orderService.GetPaymentSummaryAsync(userId, id);

                if (model == null)
                {
                    logger.LogWarning(PaymentSummaryNotFound, id, userId);
                    return NotFound();
                }

                return View(model);
            }
            catch (Exception e)
            {
                logger.LogError(e, PaymentPageLoadError, id);
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
                logger.LogWarning(UnauthenticatedPaymentAccess);
                return RedirectToAction("Login", "Account");
            }

            PaymentSummaryViewModel? summary =
                await orderService.GetPaymentSummaryAsync(userId, orderId);

            if (summary == null)
            {
                logger.LogWarning(PaymentSummaryNotFound, orderId, userId);
                return NotFound();
            }

            var products = await orderService.GetOrderProductsAsync(userId, orderId);

            if (products == null || products.Count == 0)
            {
                logger.LogWarning(OrderProductsNotFound, orderId, userId);
                return NotFound();
            }

            var domain = $"{Request.Scheme}://{Request.Host}";
            
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
            Session session;

            try
            {
                session = service.Create(options);
            }
            catch (StripeException ex)
            {
                logger.LogError(ex, StripeSessionCreationFailed, orderId);
                TempData[TempDataKeys.ErrorMessage] = InitializationFailed;

                return RedirectToAction(nameof(Payment), new { id = orderId });
            }

            await orderService.AttachStripeSessionAsync(orderId, session.Id);

            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> Success(string session_id)
        {
            string? userId = GetUserId();

            if (userId == null)
            {
                logger.LogWarning(UnauthenticatedPaymentAccess);
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(session_id) || session_id.Length > 80)
            {
                return BadRequest();
            }

            var sessionService = new SessionService();
            Session session;

            try
            {
                session = await sessionService.GetAsync(session_id);
            }
            catch (StripeException ex)
            {
                logger.LogWarning(ex, StripeSessionLookupFailed, session_id);
                return NotFound();
            }

            if (session?.Metadata == null || !session.Metadata.TryGetValue("orderId", out var orderIdStr))
            {
                logger.LogWarning(MissingOrderIdMetadata, session_id);
                return NotFound();
            }

            if (!long.TryParse(orderIdStr, out var orderId))
            {
                return NotFound();
            }

            if (!string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(Cancel), new { orderId });
            }

            var model = await orderService.GetPaymentSuccessAsync(userId, orderId);

            if (model == null)
            {
                logger.LogWarning(PaymentSuccessDataMissing, orderId, userId);
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
                logger.LogWarning(UnauthenticatedPaymentAccess);
                return RedirectToAction("Login", "Account");
            }

            PaymentSummaryViewModel? summary = await orderService.GetPaymentSummaryAsync(userId, orderId);
            
            if (summary == null)
            {
                logger.LogWarning(PaymentSummaryNotFound, orderId, userId);
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