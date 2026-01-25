using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.User;

namespace TechStore.Services.Core
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public UserService(IUserRepository userRepository, 
            UserManager<User> userManager, 
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            List<string> roles = await this.roleManager
            .Roles
            .Select(r => r.Name!)
            .ToListAsync();

            return roles;
        }

        public async Task AssignRoleAsync(Guid userId, string role)
        {
            User? user = await this.userManager.FindByIdAsync(userId.ToString());

            if (user != null)
            {
                IList<string> currentRoles = await this.userManager.GetRolesAsync(user);
                await this.userManager.RemoveFromRolesAsync(user, currentRoles);
                await this.userManager.AddToRoleAsync(user, role);
            }
        }

        public async Task<IEnumerable<UserManageViewModel>> GetPagedAsync(int page, int pageSize, Guid currentUserId)
        {
            IEnumerable<User> users = await this.userRepository
                        .GetAllAttached()
                        .Where(u => u.Id != currentUserId)
                        .OrderBy(u => u.Email)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            List<UserManageViewModel> userViewModels = new List<UserManageViewModel>();

            foreach (User user in users)
            {
                IList<string> roles = await this.userManager.GetRolesAsync(user);

                userViewModels.Add(new UserManageViewModel
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Roles = roles.ToList(),
                });
            }

            return userViewModels;
        }

        public async Task<int> GetTotalCountAsync()
        {
            int countOfUsers = await this.userRepository
                .CountAsync();

            return countOfUsers;
        }

        public async Task<DeliveryDetailsViewModel> GetDeliveryDetailsAsync(Guid userId)
        {
            User? user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new InvalidOperationException($"User not found: {userId}");
            }

            return new DeliveryDetailsViewModel
            {
                FullName = user.FullName ?? string.Empty,
                Address = user.Address ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty
            };
        }

        public async Task<bool> UpdateDeliveryDetailsAsync(Guid userId, DeliveryDetailsViewModel model)
        {
            User? user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return false;
            }

            user.FullName = model.FullName.Trim();
            user.Address = model.Address.Trim();
            user.PhoneNumber = model.PhoneNumber.Trim();

            IdentityResult result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
