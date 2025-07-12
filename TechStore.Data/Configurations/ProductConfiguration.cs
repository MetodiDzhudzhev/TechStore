using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Data.Models;
using static TechStore.GCommon.ValidationConstants.Product;

namespace TechStore.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            entity
             .HasKey(p => p.Id);

            entity
                 .Property(p => p.Name)
                 .IsRequired()
                 .HasMaxLength(NameMaxLength);

            entity
                 .Property(p => p.Description)
                 .IsRequired()
                 .HasMaxLength(DescriptionMaxLength);

            entity
                 .Property(p => p.ImageUrl)
                 .IsRequired(false);

            entity
                .Property(p => p.Price)
                .IsRequired()
                .HasColumnType(PriceSqlType);

            entity
                .Property(p => p.IsDeleted)
                .HasDefaultValue(false);

            entity
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            entity
                 .HasOne(p => p.Brand)
                 .WithMany(b => b.Products)
                 .HasForeignKey(p => p.BrandId)
                 .OnDelete(DeleteBehavior.NoAction);

            entity
                .HasQueryFilter(p => p.IsDeleted == false);
        }
    }
}
