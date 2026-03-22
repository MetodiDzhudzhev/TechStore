using TechStore.Web.ViewModels.User;

namespace TechStore.Services.Core.Interfaces
{
    public interface IUserService
    {
        Task<List<string>> GetAllRolesAsync();
        Task<bool> AssignRoleAsync(Guid userId, string role);

        Task<UserManageListViewModel> GetPagedAsync(int page, int pageSize, Guid currentUserId);
        Task<int> GetTotalCountAsync();

        Task<DeliveryDetailsViewModel> GetDeliveryDetailsAsync(Guid userId);
        Task<bool> UpdateDeliveryDetailsAsync(Guid userId, DeliveryDetailsViewModel model);
    }
}
