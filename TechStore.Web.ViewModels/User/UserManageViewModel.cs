namespace TechStore.Web.ViewModels.User
{
    public class UserManageViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string? SelectedRole { get; set; }
    }
}

