using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TechStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialDbSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "Description", "LogoUrl", "Name" },
                values: new object[,]
                {
                    { 1, "Innovative electronics and premium design.", "https://1000logos.net/wp-content/uploads/2017/02/Apple-Logosu.png", "Apple" },
                    { 2, "Leading technology company with wide range of devices.", "https://upload.wikimedia.org/wikipedia/commons/thumb/2/24/Samsung_Logo.svg/2560px-Samsung_Logo.svg.png", "Samsung" },
                    { 3, "Global leader in PCs, laptops, and smart devices.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTxyJgk2KnzsUk1zw7K-DXSzLxHrUPQUPwobA&s", "Lenovo" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Laptops" },
                    { 2, "Smartphones" },
                    { 3, "TV" },
                    { 4, "PC periphery" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CategoryId", "Description", "ImageUrl", "Name", "Price", "QuantityInStock" },
                values: new object[,]
                {
                    { new Guid("35fbca49-2d9c-478b-afeb-a95082545d14"), 3, 1, "Lenovo IdeaPad Slim 5 with Intel Core i5, 8GB RAM, 512GB SSD.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSO1LnZWrh9l_ULE65ySQ1ssY5qGmCOQc4NrA&s", "Lenovo IdeaPad Slim 5", 899.99m, 20 },
                    { new Guid("4399fcae-b322-43ed-a8e4-c96ff27ac2e8"), 2, 3, "Телевизор с QLED дисплей, 4K резолюция, HDR10+ и Smart Hub за приложения като Netflix и YouTube.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRqhUIT0G-Xg64qekomaN1yz9vQ3IuI8C9K4g&s", "Samsung 55\" QLED 4K Smart TV", 1399.99m, 10 },
                    { new Guid("57e0497f-9cab-47cc-a467-497e791c20b2"), 1, 1, "14-inch MacBook Pro with Apple M2 chip, 16GB RAM, 512GB SSD.", "https://images.hothardware.com/contentimages/article/3292/content/small_angle-2-apple-macbook-pro-14-m2-pro-2023.jpg", "Apple MacBook Pro 14 M2", 2899.99m, 10 },
                    { new Guid("736b0c5c-4482-410d-90c3-b8edd5f11e24"), 3, 1, "Lenovo Legion 5 Pro gaming laptop, AMD Ryzen 7, 16GB RAM, 1TB SSD, RTX 3070.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQPIpGxglDmGd4A1erqxXC4uA6QcN3pIJSOAg&s", "Lenovo Legion 5 Pro", 1999.99m, 15 },
                    { new Guid("8730ca23-9b23-4d43-8a3d-34693e441441"), 2, 4, "Безжични слушалки с активно шумопотискане, до 29 часа живот на батерията и страхотно качество на звука.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ9Kf2Agu1AEjnxAQ9SVmJF9ou4OsmDy4WiCQ&s", "Samsung Galaxy Buds2 Pro", 319.00m, 50 },
                    { new Guid("a0b4a61d-2cb6-44cf-bc84-63663595fa44"), 2, 2, "Мощен смартфон с 6.1\" AMOLED дисплей, 120Hz, Snapdragon 8 Gen 3 и 8GB RAM.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR24HXLuxX_OvKBnjX--FCcTXi_ThJTCZ8alg&s", "Samsung Galaxy S24", 1799.99m, 25 },
                    { new Guid("b052d75f-2ab0-42d6-a98c-b1e9f55eae50"), 1, 2, "Мощен и елегантен смартфон от Apple с A16 Bionic чип, 6.1\" Super Retina дисплей и iOS 17.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRjWapJRJu5ya98k9K8ajdbm0fNo3vB1mgSpA&s", "iPhone 15", 2199.00m, 15 },
                    { new Guid("f305d8e5-4a36-4cb3-b949-75f148c2d474"), 3, 4, "Механична клавиатура с RGB подсветка, анти-призрачни бутони и устойчив дизайн.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ5KNpwOSZt5m37tlAOfr_RJvvrGAhy8_Jugg&s", "Lenovo Legion K500 RGB", 149.00m, 30 },
                    { new Guid("fd01c89a-932d-450b-b43c-73b6c048a602"), 2, 2, "Среден клас смартфон с 6.6\" Super AMOLED дисплей, Exynos 1480 и голяма батерия.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSZBe5qwi_fXr1uyY2x7crjyR0hxXlrnyGV9w&s", "Samsung Galaxy A55", 789.99m, 20 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("35fbca49-2d9c-478b-afeb-a95082545d14"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("4399fcae-b322-43ed-a8e4-c96ff27ac2e8"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("57e0497f-9cab-47cc-a467-497e791c20b2"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("736b0c5c-4482-410d-90c3-b8edd5f11e24"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("8730ca23-9b23-4d43-8a3d-34693e441441"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a0b4a61d-2cb6-44cf-bc84-63663595fa44"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b052d75f-2ab0-42d6-a98c-b1e9f55eae50"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f305d8e5-4a36-4cb3-b949-75f148c2d474"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("fd01c89a-932d-450b-b43c-73b6c048a602"));

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
