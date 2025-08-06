using Microsoft.EntityFrameworkCore;

namespace TechStore.Data.Models
{
    [Comment("Category of products in the system")]
    public class Category
    {
        [Comment("Category identifier")]
        public int Id { get; set; }


        [Comment("Category name")]
        public string Name { get; set; } = null!;


        [Comment("Image URL representing the category")]
        public string? ImageUrl { get; set; }


        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();


        [Comment("Shows if the category is deleted")]
        public bool IsDeleted { get; set; }
    }
} 
