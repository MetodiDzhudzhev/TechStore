using Microsoft.EntityFrameworkCore;
namespace TechStore.Data.Models
{
    
    [Comment("Brand entity")]
    public class Brand
    {

        [Comment("Brand identifier")]
        public int Id { get; set; }


        [Comment("Brand name")]
        public string Name { get; set; } = null!;


        [Comment("URL of the brand logo")]
        public string? LogoUrl { get; set; }


        [Comment("Description of the brand")]
        public string? Description { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();


        [Comment("Shows if the brand is deleted")]
        public bool IsDeleted { get; set; }
    }
}
