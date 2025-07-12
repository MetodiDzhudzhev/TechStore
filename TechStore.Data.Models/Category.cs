namespace TechStore.Data.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();

        public bool IsDeleted { get; set; }
    }
}
