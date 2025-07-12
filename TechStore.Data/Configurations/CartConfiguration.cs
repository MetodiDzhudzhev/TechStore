using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Data.Models;

namespace TechStore.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> entity)
        {
            entity
             .HasKey(c => c.Id);

            entity
                 .HasOne(c => c.User)
                 .WithOne(u => u.Cart)
                 .HasForeignKey<Cart>(c => c.Id) // Both - Cart and User are with the same ID due to One to One relation.
                 .OnDelete(DeleteBehavior.Cascade);

            entity
                .Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            entity
                .HasQueryFilter(c => c.IsDeleted == false
                                  && c.User.IsDeleted == false);
        }
    }
}
