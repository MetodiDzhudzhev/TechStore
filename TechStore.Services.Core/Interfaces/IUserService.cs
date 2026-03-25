using TechStore.Web.ViewModels.User;

namespace TechStore.Services.Core.Interfaces
{
    public interface IUserService
    {
        Task<bool> AssignRoleAsync(Guid userId, string role);
        Task<UserManageListViewModel> GetPagedAsync(int page, int pageSize, Guid currentUserId);
        Task<DeliveryDetailsViewModel?> GetDeliveryDetailsAsync(Guid userId);
        Task<bool> UpdateDeliveryDetailsAsync(Guid userId, DeliveryDetailsViewModel model);
    }
}
