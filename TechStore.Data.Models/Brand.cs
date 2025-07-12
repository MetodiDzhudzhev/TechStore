namespace TechStore.Data.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();

        public bool IsDeleted { get; set; }
    }
}
