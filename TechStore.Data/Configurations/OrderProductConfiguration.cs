using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Data.Models;

namespace TechStore.Data.Configurations
{
    public class OrderProductConfiguration : IEntityTypeConfiguration<OrderProduct>
    {
        public void Configure(EntityTypeBuilder<OrderProduct> entity)
        {
            entity
             .HasKey(op => new { op.OrderId, op.ProductId });

            entity
                 .Property(op => op.Quantity)
                 .IsRequired();

            entity
                 .Property(op => op.UnitPrice)
                 .IsRequired()
                 .HasColumnType("decimal(18,2)");

            entity
                 .HasOne(op => op.Order)
                 .WithMany(o => o.OrdersProducts)
                 .HasForeignKey(op => op.OrderId)
                 .OnDelete(DeleteBehavior.NoAction);

            entity
                 .HasOne(op => op.Product)
                 .WithMany(p => p.OrdersProducts)
                 .HasForeignKey(op => op.ProductId)
                 .OnDelete(DeleteBehavior.NoAction);

            entity
                .HasQueryFilter(op => op.Product.IsDeleted == false
                                   && op.Order.User.IsDeleted == false);
        }
    }
}
