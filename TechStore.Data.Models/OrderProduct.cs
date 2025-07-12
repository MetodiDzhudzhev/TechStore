namespace TechStore.Data.Models
{
    public class OrderProduct
    {
        public long OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
