using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Data.Models;

namespace TechStore.Data.Configurations
{
    public class CartProductConfiguration : IEntityTypeConfiguration<CartProduct>
    {
        public void Configure(EntityTypeBuilder<CartProduct> entity)
        {
            entity
             .HasKey(cp => new { cp.CartId, cp.ProductId });

            entity
                 .Property(cp => cp.Quantity)
                 .IsRequired()
                 .HasDefaultValue(1);

            entity
                 .HasOne(cp => cp.Cart)
                 .WithMany(c => c.Products)
                 .HasForeignKey(cp => cp.CartId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity
                 .HasOne(cp => cp.Product)
                 .WithMany(p => p.CartsProducts)
                 .HasForeignKey(cp => cp.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasQueryFilter(cp => cp.Product.IsDeleted == false
                                   && cp.Cart.IsDeleted == false);
        }
    }
}
