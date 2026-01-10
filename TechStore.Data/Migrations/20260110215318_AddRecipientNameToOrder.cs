using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipientNameToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "The name of the person who will receive the delivery");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientName",
                table: "Orders");
        }
    }
}
