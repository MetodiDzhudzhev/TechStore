using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechStore.Data.Models.Enums;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Order;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                OrderEditPageViewModel? page = await orderService.GetEditPageAsync(id);

                if (page == null)
                {
                    return NotFound();
                }

                ViewBag.AllowedStatuses = ToSelectListItems(page.AllowedStatuses);
                return View(page);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while preparing Edit order form for order with Id {OrderId}!", id);
                TempData["ErrorMessage"] = "An error occurred while preparing the Edit order form.";
                return RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ChangeStatus([Bind(Prefix = "Status")] OrderEditStatusInputModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    OrderEditPageViewModel? page = await orderService.GetEditPageAsync(model.Id);

                    if (page == null)
                    {
                        logger.LogWarning("Order with Id {OrderId} was not found.", model.Id);
                        return NotFound();
                    }

                    page.Status.NewStatus = model.NewStatus;

                    ViewBag.AllowedStatuses = ToSelectListItems(page.AllowedStatuses);
                    return View("Edit", page);
                }

                bool success = await orderService.EditStatusAsync(model.Id, model.NewStatus);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Invalid status transition or order is locked.");

                    OrderEditPageViewModel? page = await orderService.GetEditPageAsync(model.Id);
                    if (page == null)
                    {
                        logger.LogWarning("Order with Id {OrderId} was not found.", model.Id);
                        return NotFound();
                    }

                    page.Status.NewStatus = model.NewStatus;

                    ViewBag.AllowedStatuses = ToSelectListItems(page.AllowedStatuses);
                    return View("Edit", page);
                }

                TempData["SuccessMessage"] = "Order status updated successfully.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while changing status for order with Id {OrderId}.", model.Id);
                TempData["ErrorMessage"] = "An unexpected error occurred while changing the order status.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> EditShipping([Bind(Prefix = "Shipping")] OrderEditShippingDetailsInputModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    OrderEditPageViewModel? page = await orderService.GetEditPageAsync(model.Id);

                    if (page == null)
                    {
                        logger.LogWarning("Order with Id {OrderId} was not found.", model.Id);
                        return NotFound();
                    }

                    page.Shipping = model;

                    ViewBag.AllowedStatuses = ToSelectListItems(page.AllowedStatuses);
                    return View("Edit", page);
                }

                bool success = await orderService.EditShippingDetailsAsync(model);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Shipping details cannot be updated. The order may be locked.");

                    OrderEditPageViewModel? page = await orderService.GetEditPageAsync(model.Id);

                    if (page == null)
                    {
                        logger.LogWarning("Order with Id {OrderId} was not found.", model.Id);
                        return NotFound();
                    }

                    page.Shipping = model;
                    ViewBag.AllowedStatuses = ToSelectListItems(page.AllowedStatuses);

                    return View("Edit", page);
                }

                TempData["SuccessMessage"] = "Shipping details updated successfully.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while editing shipping details for order with Id {OrderId}.", model.Id);
                TempData["ErrorMessage"] = "An unexpected error occurred while updating shipping details.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage(int page = 1)
        {
            string? userId = this.GetUserId();

            if (!Guid.TryParse(userId, out Guid currentUserId))
            {
                return Unauthorized();
            }

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

    }
}
