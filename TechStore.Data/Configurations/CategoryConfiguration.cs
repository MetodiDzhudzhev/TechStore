using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Data.Models;
using static TechStore.GCommon.ValidationConstants.Category;

namespace TechStore.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity
             .HasKey(c => c.Id);

            entity
                 .Property(c => c.Name)
                 .IsRequired()
                 .HasMaxLength(NameMaxLength);

            entity
                .Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            entity
                 .HasIndex(c => c.Name)
                 .IsUnique();

            entity
                .HasQueryFilter(c => c.IsDeleted == false);
        }
    }
}
