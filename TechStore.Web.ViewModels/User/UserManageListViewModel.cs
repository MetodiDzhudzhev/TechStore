namespace TechStore.Web.ViewModels.User
{
    public class UserManageListViewModel
    {
        public IEnumerable<UserManageViewModel> Users { get; set; } = new List<UserManageViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<string> AllRoles { get; set; } = new();
    }
}
