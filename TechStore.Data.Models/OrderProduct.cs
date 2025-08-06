using Microsoft.EntityFrameworkCore;

namespace TechStore.Data.Models
{
    [Comment("Association entity between Order and Product")]
    public class OrderProduct
    {
        [Comment("Foreign key to the related Order")]
        public long OrderId { get; set; }

        public virtual Order Order { get; set; } = null!;


        [Comment("Foreign key to the related Product")]
        public Guid ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;


        [Comment("Quantity of the product ordered.")]
        public int Quantity { get; set; }


        [Comment("Unit price of the product at the time of the order.")]
        public decimal UnitPrice { get; set; }
    }
}
