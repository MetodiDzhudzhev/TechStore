using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechStore.Services.Core.Interfaces;

namespace TechStore.Web.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly ICartService cartService;

        public CartCountViewComponent(ICartService cartService)
        {
            this.cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return View(0);
            }

            string? userIdString = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return View(0);
            }

            int count = await cartService.GetCartItemsCountAsync(userId);

            return View(count);
        }
    }
}
