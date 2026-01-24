using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;

namespace TechStore.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class AccountController : BaseAccountController
    {
        private readonly IOrderService orderService;

        private const int PageSize = 5;
        
        public AccountController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders(int page = 1)
        {
            string? userId = this.GetUserId();

            if (!Guid.TryParse(userId, out Guid currentUserId))
            {
                return Unauthorized();
            }

            var viewModel = await orderService.GetMyOrdersPagedAsync(currentUserId, page, PageSize);
            return View(viewModel);
        }
    }
}
