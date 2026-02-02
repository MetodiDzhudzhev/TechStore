using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    [Authorize(Roles = "Admin,Manager")]
    public class HomeController : BaseControlPanelController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
