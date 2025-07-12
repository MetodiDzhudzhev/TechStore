using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Data.Models;
using static TechStore.GCommon.ValidationConstants.Brand;

namespace TechStore.Data.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> entity)
        {
            entity
             .HasKey(b => b.Id);

            entity
                 .Property(b => b.Name)
                 .IsRequired()
                 .HasMaxLength(NameMaxLength);

            entity
                 .HasIndex(b => b.Name)
                 .IsUnique();

            entity
                 .Property(b => b.LogoUrl)
                 .IsRequired(false);

            entity
                 .Property(b => b.Description)
                 .IsRequired(false)
                 .HasMaxLength(DescriptionMaxLength);

            entity
                .Property(b => b.IsDeleted)
                .HasDefaultValue(false);

            entity
                .HasQueryFilter(b => b.IsDeleted == false);
        }
    }
}
