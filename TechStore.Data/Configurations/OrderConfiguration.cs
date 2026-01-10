using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using static TechStore.GCommon.ValidationConstants.Order;

namespace TechStore.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity
             .HasKey(o => o.Id);

            entity
                 .Property(o => o.OrderDate)
                 .IsRequired();

            entity
                 .Property(o => o.ShippingAddress)
                 .IsRequired()
                 .HasMaxLength(ShippingAddressMaxLength);

            entity
                 .Property(o => o.Status)
                 .HasConversion<string>()
                 .IsRequired();

            entity
                .Property(o => o.PaymentMethod)
                .HasConversion<string>()
                .IsRequired();

            entity
                 .HasOne(o => o.User)
                 .WithMany(u => u.Orders)
                 .HasForeignKey(o => o.UserId)
                 .OnDelete(DeleteBehavior.NoAction);

            entity
                 .Property(o => o.RecipientName)
                 .IsRequired()
                 .HasMaxLength(RecipientNameMaxLength);

            entity
                .Property(o => o.PhoneNumber)
                .IsRequired()
                .HasMaxLength(PhoneNumberMaxLength);

            entity
                .Property(o => o.Email)
                .IsRequired()
                .HasMaxLength(EmailMaxLength);

            entity
                 .Ignore(o => o.TotalAmount);

            entity
                .HasQueryFilter(o => o.User.IsDeleted == false);
        }
    }
}
