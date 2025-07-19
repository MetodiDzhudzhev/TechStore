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
                 .Property(c => c.ImageUrl)
                 .IsRequired(false);

            entity
                .HasQueryFilter(c => c.IsDeleted == false);

            entity
                .HasData(this.GenerateSeedCategories());
        }

        private List<Category> GenerateSeedCategories()
        {
            List<Category> seedCategories = new List<Category>()
            {
                new Category
                {
                    Id = 1,
                    Name = "Laptops"
                },

                new Category
                {
                    Id = 2,
                    Name = "Smartphones"
                },

                new Category
                {
                    Id = 3,
                    Name = "TV"
                },

                new Category
                {
                    Id = 4,
                    Name = "PC periphery"
                }
            };

            return seedCategories;
        }
    }
}
