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

            entity
                .HasData(this.GenerateSeedProducts());
        }

        private List<Product> GenerateSeedProducts()
        {
            List<Product> seedProducts = new List<Product>()
            {
                new Product
                {
                   Id = Guid.Parse("57E0497F-9CAB-47CC-A467-497E791C20B2"),
                   Name = "Apple MacBook Pro 14 M2",
                   Description = "14-inch MacBook Pro with Apple M2 chip, 16GB RAM, 512GB SSD.",
                   ImageUrl = "https://images.hothardware.com/contentimages/article/3292/content/small_angle-2-apple-macbook-pro-14-m2-pro-2023.jpg",
                   Price = 2899.99m,
                   QuantityInStock = 10,
                   CategoryId = 1,
                   BrandId = 1
                },

                new Product
                {
                    Id = Guid.Parse("736B0C5C-4482-410D-90C3-B8EDD5F11E24"),
                    Name = "Lenovo Legion 5 Pro",
                    Description = "Lenovo Legion 5 Pro gaming laptop, AMD Ryzen 7, 16GB RAM, 1TB SSD, RTX 3070.",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQPIpGxglDmGd4A1erqxXC4uA6QcN3pIJSOAg&s",
                    Price = 1999.99m,
                    QuantityInStock = 15,
                    CategoryId = 1,
                    BrandId = 3
                },

                new Product
                {
                    Id = Guid.Parse("35FBCA49-2D9C-478B-AFEB-A95082545D14"),
                    Name = "Lenovo IdeaPad Slim 5",
                    Description = "Lenovo IdeaPad Slim 5 with Intel Core i5, 8GB RAM, 512GB SSD.",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSO1LnZWrh9l_ULE65ySQ1ssY5qGmCOQc4NrA&s",
                    Price = 899.99m,
                    QuantityInStock = 20,
                    CategoryId = 1,
                    BrandId = 3
                },

                new Product
                {
                    Id = Guid.Parse("A0B4A61D-2CB6-44CF-BC84-63663595FA44"),
                    Name = "Samsung Galaxy S24",
                    Description = "Мощен смартфон с 6.1\" AMOLED дисплей, 120Hz, Snapdragon 8 Gen 3 и 8GB RAM.",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR24HXLuxX_OvKBnjX--FCcTXi_ThJTCZ8alg&s",
                    Price = 1799.99m,
                    QuantityInStock = 25,
                    CategoryId = 2,
                    BrandId = 2
                },

                new Product
                {
                    Id = Guid.Parse("FD01C89A-932D-450B-B43C-73B6C048A602"),
                    Name = "Samsung Galaxy A55",
                    Description = "Среден клас смартфон с 6.6\" Super AMOLED дисплей, Exynos 1480 и голяма батерия.",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSZBe5qwi_fXr1uyY2x7crjyR0hxXlrnyGV9w&s",
                    Price = 789.99m,
                    QuantityInStock = 20,
                    CategoryId = 2,
                    BrandId = 2
                },

                new Product
                {
                    Id = Guid.Parse("B052D75F-2AB0-42D6-A98C-B1E9F55EAE50"),
                    Name = "iPhone 15",
                    Description = "Мощен и елегантен смартфон от Apple с A16 Bionic чип, 6.1\" Super Retina дисплей и iOS 17.",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRjWapJRJu5ya98k9K8ajdbm0fNo3vB1mgSpA&s",
                    Price = 2199.00m,
                    QuantityInStock = 15,
                    CategoryId = 2,
                    BrandId = 1
                },

                new Product
                {
                    Id = Guid.Parse("4399FCAE-B322-43ED-A8E4-C96FF27AC2E8"),
                    Name = "Samsung 55\" QLED 4K Smart TV",
                    Description = "Телевизор с QLED дисплей, 4K резолюция, HDR10+ и Smart Hub за приложения като Netflix и YouTube.",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRqhUIT0G-Xg64qekomaN1yz9vQ3IuI8C9K4g&s",
                    Price = 1399.99m,
                    QuantityInStock = 10,
                    CategoryId = 3,
                    BrandId = 2
                },

                new Product
                {
                    Id = Guid.Parse("8730CA23-9B23-4D43-8A3D-34693E441441"),
                    Name = "Samsung Galaxy Buds2 Pro",
                    Description = "Безжични слушалки с активно шумопотискане, до 29 часа живот на батерията и страхотно качество на звука.",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ9Kf2Agu1AEjnxAQ9SVmJF9ou4OsmDy4WiCQ&s",
                    Price = 319.00m,
                    QuantityInStock = 50,
                    CategoryId = 4,
                    BrandId = 2
                },

                new Product
                {
                    Id = Guid.Parse("F305D8E5-4A36-4CB3-B949-75F148C2D474"),
                    Name = "Lenovo Legion K500 RGB",
                    Description = "Механична клавиатура с RGB подсветка, анти-призрачни бутони и устойчив дизайн.",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ5KNpwOSZt5m37tlAOfr_RJvvrGAhy8_Jugg&s",
                    Price = 149.00m,
                    QuantityInStock = 30,
                    CategoryId = 4,
                    BrandId = 3
                }
            };

            return seedProducts;
        }
    }
}
