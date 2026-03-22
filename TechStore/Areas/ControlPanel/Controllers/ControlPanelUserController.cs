using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.User;

using TechStore.GCommon;
using UserLog = TechStore.GCommon.LogMessages.User;
using UserUi = TechStore.GCommon.UiMessages.User;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    public class ControlPanelUserController : BaseControlPanelController
    {
        private readonly IUserService userService;

        private readonly ILogger<ControlPanelUserController> logger;

        private const int PageSize = 5;

        public ControlPanelUserController(IUserService userService, 
            ILogger<ControlPanelUserController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }


        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Manage(int page = 1)
        {

            string? userId = this.GetUserId();

            if (!Guid.TryParse(userId, out Guid currentUserId))
            {
                return Unauthorized();
            }

            UserManageListViewModel viewModel = await this.userService.GetPagedAsync(page, PageSize, currentUserId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssignRole(Guid userId, string selectedRole, int currentPage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(selectedRole))
                {
                    logger.LogWarning(UserLog.RoleNotSelected, userId);
                    TempData[TempDataKeys.ErrorMessage] = UserUi.RoleNotSelected;

                    return RedirectToAction(nameof(Manage), new { page = currentPage });
                }

                bool result = await userService.AssignRoleAsync(userId, selectedRole);

                if (!result)
                {
                    logger.LogWarning(UserLog.RoleAssignFailed, userId, selectedRole);
                    TempData[TempDataKeys.ErrorMessage] = UserUi.RoleAssignFailed;

                    return RedirectToAction(nameof(Manage), new { page = currentPage });
                }

                logger.LogInformation(UserLog.RoleAssignSuccess, userId, selectedRole);
                TempData[TempDataKeys.SuccessMessage] = UserUi.RoleAssignSuccess;

                return RedirectToAction(nameof(Manage), new { page = currentPage });
            }
            catch (Exception e)
            {
                logger.LogError(e, UserLog.RoleAssignError, userId, selectedRole);
                TempData[TempDataKeys.ErrorMessage] = UserUi.RoleAssignError;

                return RedirectToAction(nameof(Manage), new { page = currentPage });
            }
        }
    }
}