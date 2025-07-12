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

            entity
                .HasData(this.GenerateSeedBrands());
        }

        private List<Brand> GenerateSeedBrands()
        {
            List<Brand> seedBrands = new List<Brand>()
            {
                new Brand
                {
                    Id = 1,
                    Name = "Apple",
                    LogoUrl = "https://1000logos.net/wp-content/uploads/2017/02/Apple-Logosu.png",
                    Description = "Innovative electronics and premium design."
                },

                new Brand
                {
                    Id = 2,
                    Name = "Samsung",
                    LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/24/Samsung_Logo.svg/2560px-Samsung_Logo.svg.png",
                    Description = "Leading technology company with wide range of devices."
                },

                new Brand
                {
                    Id = 3,
                    Name = "Lenovo",
                    LogoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTxyJgk2KnzsUk1zw7K-DXSzLxHrUPQUPwobA&s",
                    Description = "Global leader in PCs, laptops, and smart devices."
                }
            };

            return seedBrands;
        }
    }
}
