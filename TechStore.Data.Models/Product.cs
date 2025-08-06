using Microsoft.EntityFrameworkCore;

namespace TechStore.Data.Models
{
    [Comment("Product in the system")]
    public class Product
    {
        [Comment("Product identifier")]
        public Guid Id { get; set; }


        [Comment("Product name")]
        public string Name { get; set; } = null!;


        [Comment("Description of the product")]
        public string Description { get; set; } = null!;


        [Comment("Image URL for the product")]
        public string? ImageUrl { get; set; }


        [Comment("Price of the product")]
        public decimal Price { get; set; }


        [Comment("Quantity of the product available in stock")]
        public int QuantityInStock { get; set; }


        [Comment("Foreign key to the related category")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;


        [Comment("Foreign key to the related brand")]
        public int BrandId { get; set; }

        public virtual Brand Brand { get; set; } = null!;


        [Comment("Shows if the product is deleted")]
        public bool IsDeleted { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public virtual ICollection<OrderProduct> OrdersProducts { get; set; } = new HashSet<OrderProduct>();
        public virtual ICollection<CartProduct> CartsProducts { get; set; } = new HashSet<CartProduct>();
    }
}
