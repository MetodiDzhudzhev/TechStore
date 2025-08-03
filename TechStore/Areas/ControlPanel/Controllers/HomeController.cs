using Microsoft.AspNetCore.Mvc;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    public class HomeController : BaseControlPanelController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
