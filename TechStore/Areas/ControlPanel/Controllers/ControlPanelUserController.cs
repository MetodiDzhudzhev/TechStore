using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.User;

namespace TechStore.Web.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    public class ControlPanelUserController : BaseControlPanelController
    {
        private readonly IUserService userService;

        private readonly ILogger<ControlPanelUserController> logger;

        private const int PageSize = 10;

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
            int totalUsers = await userService.GetTotalCountAsync();
            int totalPages = (int)Math.Ceiling((double)totalUsers / PageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (page > totalPages && totalPages > 0)
            {
                page = totalPages;
            }

            string userId = this.GetUserId();

            if (!Guid.TryParse(userId, out Guid currentUserId))
            {
                return Unauthorized();
            }

            IEnumerable<UserManageViewModel> users = await this.userService.GetPagedAsync(page, PageSize, currentUserId);

            List<string> allRoles = await this.userService.GetAllRolesAsync();

            UserManageListViewModel viewModel = new UserManageListViewModel
            {
                Users = users,
                AllRoles = allRoles,
                CurrentPage = page,
                TotalPages = totalPages,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssignRole(Guid userId, string selectedRole, int currentPage)
        {
            if (string.IsNullOrWhiteSpace(selectedRole))
            {
                ModelState.AddModelError("", "Please, select a role.");
            }
            else
            {
                await this.userService.AssignRoleAsync(userId, selectedRole);
                logger.LogInformation("Role '{Role}' assigned to user {UserId}", selectedRole, userId);
            }

            return RedirectToAction(nameof(Manage), new { page = currentPage });
        }
    }
}
