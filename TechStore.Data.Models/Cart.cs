namespace TechStore.Data.Models
{
    public class Cart
    {
        //Cart and User are with the same Id, due to the One to One relation
        public Guid Id { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual ICollection<CartProduct> Products { get; set; } = new HashSet<CartProduct>();

        public bool IsDeleted { get; set; }
    }
}
