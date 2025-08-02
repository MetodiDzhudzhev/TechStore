using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechStore.Web.ViewModels;

namespace TechStore.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            switch (statusCode)
            {
                case 401:
                    return this.View("UnauthorizedError");
                case 403:
                    return this.View("ForbiddenError");
                case 404:
                    return this.View("NotFoundError");
                case 500:
                case null:
                    return this.View("InternalServerError");
                default:
                    return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}
