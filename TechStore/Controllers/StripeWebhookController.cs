using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using TechStore.Services.Core.Interfaces;

[ApiController]
[Route("api/stripe/webhook")]
public class StripeWebhookController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IOrderService orderService;
    private readonly ILogger<StripeWebhookController> logger;

    public StripeWebhookController(
        IConfiguration configuration,
        IOrderService orderService,
        ILogger<StripeWebhookController> logger)
    {
        this.configuration = configuration;
        this.orderService = orderService;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        string secret = configuration["Stripe:WebhookSecret"]!;

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                secret);

            logger.LogInformation("Stripe webhook received. Type={Type}", stripeEvent.Type);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Stripe webhook signature validation failed.");
            return BadRequest();
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;

            if (session?.PaymentStatus != "paid")
            {
                logger.LogInformation("Checkout completed but not paid. SessionId={SessionId}, PaymentStatus={Status}",
                    session?.Id, session?.PaymentStatus);
                return Ok();
            }

            var orderIdStr = session?.Metadata?["orderId"];
            if (long.TryParse(orderIdStr, out var orderId))
            {
                await orderService.MarkOrderAsPaidByOrderIdAsync(orderId);
                logger.LogInformation("Order {OrderId} marked as paid by webhook.", orderId);
            }
        }

        return Ok();
    }
}
