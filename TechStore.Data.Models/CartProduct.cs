namespace TechStore.Data.Models
{
    public class CartProduct
    {
        public Guid CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
