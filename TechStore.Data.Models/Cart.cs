using Microsoft.EntityFrameworkCore;

namespace TechStore.Data.Models
{
    [Comment("Shopping cart of a user")]
    public class Cart
    {
        [Comment("Cart identifier, same as User Id")] 
        public Guid Id { get; set; } //Cart and User are with the same Id, due to the One to One relation


        [Comment("Foreign key to the User who owns this cart")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<CartProduct> Products { get; set; } = new HashSet<CartProduct>();


        [Comment("Shows if the cart is deleted")]
        public bool IsDeleted { get; set; }
    }
}
