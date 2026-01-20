using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models.Enums;

namespace TechStore.Data.Models
{
    [Comment("Order placed by a user")]
    public class Order
    {
        [Comment("Order identifier")]
        public long Id { get; set; }


        [Comment("Date when the order was placed")]
        public DateTime OrderDate { get; set; }


        [Comment("Shipping address for the order")]
        public string ShippingAddress { get; set; } = null!;


        [Comment("Current status of the order")]
        public Status Status { get; set; }

        [Comment("Payment method for the order")]
        public PaymentMethod PaymentMethod { get; set; } 


        [Comment("Foreign key to the User who placed the order")]
        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;

        [Comment("The name of the person who will receive the delivery")]
        public string RecipientName { get; set; } = null!;

        [Comment("Contact phone number for the delivery")]
        public string PhoneNumber { get; set; } = null!;

        [Comment("Contact email for the delivery")]
        public string Email { get; set; } = null!;

        public virtual ICollection<OrderProduct> OrdersProducts { get; set; } = new HashSet<OrderProduct>();


        [Comment("Total amount for the order")]
        public decimal TotalAmount => OrdersProducts.Sum(op => op.UnitPrice * op.Quantity);

        [Comment("Stripe session identifier")]
        public string? StripeSessionId { get; set; }
    }
}
