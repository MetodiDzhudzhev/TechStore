namespace TechStore.Web.ViewModels.Brand
{
    public class BrandDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public string? logoUrl { get; set; }
    }
}
