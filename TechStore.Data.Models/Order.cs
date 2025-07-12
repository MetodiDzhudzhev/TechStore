using TechStore.Data.Models.Enums;

namespace TechStore.Data.Models
{
    public class Order
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public Status Status { get; set; }    // Pending, Shipped, Delivered, Cancelled 

        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public virtual ICollection<OrderProduct> OrdersProducts { get; set; } = new HashSet<OrderProduct>();

        public decimal TotalAmount => OrdersProducts.Sum(op => op.UnitPrice * op.Quantity);
    }
}
