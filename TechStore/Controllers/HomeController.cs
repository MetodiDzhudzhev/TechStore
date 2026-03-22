using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechStore.Web.ViewModels;

using static TechStore.GCommon.LogMessages.Home;

namespace TechStore.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> logger;
        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            logger.LogWarning(ErrorStatusCode, statusCode);

            switch (statusCode)
            {
                case 401:
                    return this.View("~/Views/Shared/Errors/UnauthorizedError.cshtml");
                case 403:
                    return this.View("~/Views/Shared/Errors/ForbiddenError.cshtml");
                case 404:
                    return this.View("~/Views/Shared/Errors/NotFoundError.cshtml");
                case 500:
                case null:
                    return this.View("~/Views/Shared/Errors/InternalServerError.cshtml");
                default:
                    return this.View("~/Views/Shared/Errors/Error.cshtml", 
                        new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}
