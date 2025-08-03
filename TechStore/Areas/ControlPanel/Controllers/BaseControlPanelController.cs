using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    [Authorize]
    public abstract class BaseControlPanelController : Controller
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
