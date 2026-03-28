using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechStore.Data.Models.Enums;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Order;

using TechStore.GCommon;
using OrderLog = TechStore.GCommon.LogMessages.Order;
using OrderUi = TechStore.GCommon.UiMessages.Order;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    [Authorize(Roles = "Admin,Manager")]
    public class ControlPanelOrderController : BaseControlPanelController
    {
        private readonly IOrderService orderService;
        private readonly ILogger<ControlPanelOrderController> logger;

        private const int PageSize = 5;

        public ControlPanelOrderController(IOrderService orderService,
            ILogger<ControlPanelOrderController> logger)
        {
            this.orderService = orderService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                OrderEditPageViewModel? page = await orderService.GetEditPageAsync(id);

                if (page == null)
                {
                    return OrderNotFound(id);
                }

                ViewBag.AllowedStatuses = ToSelectListItems(page.AllowedStatuses);
                return View(page);
            }
            catch (Exception e)
            {
                logger.LogError(e, OrderLog.EditOrderPageLoadError, id);
                TempData[TempDataKeys.ErrorMessage] = OrderUi.EditOrderPageLoadError;
                return RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus([Bind(Prefix = "Status")] OrderEditStatusInputModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    OrderEditPageViewModel? page = await orderService.GetEditPageAsync(model.Id);

                    if (page == null)
                    {
                        return OrderNotFound(model.Id);
                    }

                    page.Status.NewStatus = model.NewStatus;

                    ViewBag.AllowedStatuses = ToSelectListItems(page.AllowedStatuses);
                    return View("Edit", page);
                }

                bool success = await orderService.EditStatusAsync(model.Id, model.NewStatus);

                if (!success)
                {
                    logger.LogWarning(OrderLog.EditStatusFailed, model.Id);
                    ModelState.AddModelError(string.Empty, OrderUi.EditStatusFailed);

                    OrderEditPageViewModel? page = await orderService.GetEditPageAsync(model.Id);
                    if (page == null)
                    {
                        return OrderNotFound(model.Id);
                    }

                    page.Status.NewStatus = model.NewStatus;

                    ViewBag.AllowedStatuses = ToSelectListItems(page.AllowedStatuses);
                    return View("Edit", page);
                }

                logger.LogInformation(OrderLog.EditStatusSuccess, model.Id);
                TempData[TempDataKeys.SuccessMessage] = OrderUi.EditStatusSuccess;
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
            catch (Exception e)
            {
                logger.LogError(e, OrderLog.EditStatusError, model.Id);
                TempData[TempDataKeys.ErrorMessage] = OrderUi.EditStatusError;
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditShipping([Bind(Prefix = "Shipping")] OrderEditShippingDetailsInputModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    OrderEditPageViewModel? page = await orderService.GetEditPageAsync(model.Id);

                    if (page == null)
                    {
                        return OrderNotFound(model.Id);
                    }

                    page.Shipping = model;

                    ViewBag.AllowedStatuses = ToSelectListItems(page.AllowedStatuses);
                    return View("Edit", page);
                }

                bool success = await orderService.EditShippingDetailsAsync(model);

                if (!success)
                {
                    logger.LogWarning(OrderLog.EditShippingDetailsFailed, model.Id);
                    ModelState.AddModelError(string.Empty, OrderUi.EditShippingDetailsFailed);

                    OrderEditPageViewModel? page = await orderService.GetEditPageAsync(model.Id);

                    if (page == null)
                    {
                        return OrderNotFound(model.Id);
                    }

                    page.Shipping = model;
                    ViewBag.AllowedStatuses = ToSelectListItems(page.AllowedStatuses);

                    return View("Edit", page);
                }

                logger.LogInformation(OrderLog.EditShippingDetailsSuccess, model.Id);
                TempData[TempDataKeys.SuccessMessage] = OrderUi.EditShippingDetailsSuccess;
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
            catch (Exception e)
            {
                logger.LogError(e, OrderLog.EditShippingDetailsError, model.Id);
                TempData[TempDataKeys.ErrorMessage] = OrderUi.EditShippingDetailsError;
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int page = 1)
        {
            var viewModel = await orderService.GetManageOrdersPageAsync(page, PageSize);
            return View(viewModel);
        }

        private static IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Status> statuses)
        {
            return statuses.Select(status => new SelectListItem
            {
                Value = ((int)status).ToString(),
                Text = status.ToString()
            });
        }

        private IActionResult OrderNotFound(long orderId)
        {
            logger.LogWarning(OrderLog.NotFound, orderId);
            TempData[TempDataKeys.ErrorMessage] = OrderUi.NotFound;
            return NotFound();
        }
    }
}