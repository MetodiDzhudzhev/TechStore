using Microsoft.AspNetCore.Mvc;
using TechStore.Web.ViewModels.Common;

namespace TechStore.Web.ViewComponents
{
    public class CardGridViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<CardItemViewModel> items)
        {
            return View(items);
        }
    }
}
