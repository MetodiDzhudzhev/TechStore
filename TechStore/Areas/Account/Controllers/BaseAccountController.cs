using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TechStore.Web.Areas.Account.Controllers
{
    [Area("Account")]
    [Authorize]
    public abstract class BaseAccountController : Controller
    {
        private bool IsUserAuthenticated()
        {
            return this.User.Identity?.IsAuthenticated ?? false;
        }

        protected string? GetUserId()
        {
            string? userId = null;
            bool isAuthenticated = this.IsUserAuthenticated();

            if (isAuthenticated)
            {
                userId = this.User
                            .FindFirstValue(ClaimTypes.NameIdentifier);
            }
            return userId;
        }
    }
}
