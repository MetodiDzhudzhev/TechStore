namespace TechStore.Web.ViewModels.Brand
{
    public class BrandManageViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}