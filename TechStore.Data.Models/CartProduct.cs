using Microsoft.EntityFrameworkCore;

namespace TechStore.Data.Models
{
    [Comment("Association entity between Cart and Product")]
    public class CartProduct
    {
        [Comment("Foreign key to the related Cart")]
        public Guid CartId { get; set; }

        public virtual Cart Cart { get; set; } = null!;


        [Comment("Foreign key to the related Product")]
        public Guid ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;


        [Comment("Quantity of the product in the cart.")]
        public int Quantity { get; set; }
    }
}
