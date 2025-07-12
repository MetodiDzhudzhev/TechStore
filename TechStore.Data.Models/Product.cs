namespace TechStore.Data.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; } = null!;

        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public virtual ICollection<OrderProduct> OrdersProducts { get; set; } = new HashSet<OrderProduct>();
        public virtual ICollection<CartProduct> CartsProducts { get; set; } = new HashSet<CartProduct>();

        public bool IsDeleted { get; set; }
    }
}
