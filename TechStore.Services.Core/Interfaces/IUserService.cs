using TechStore.Web.ViewModels.User;

namespace TechStore.Services.Core.Interfaces
{
    public interface IUserService
    {
        Task<List<string>> GetAllRolesAsync();
        Task AssignRoleAsync(Guid userId, string role);

        Task<IEnumerable<UserManageViewModel>> GetPagedAsync(int page, int pageSize, Guid currentUserId);
        Task<int> GetTotalCountAsync();
    }
}
