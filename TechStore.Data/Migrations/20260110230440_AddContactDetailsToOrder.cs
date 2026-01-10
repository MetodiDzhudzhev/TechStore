using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContactDetailsToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Orders",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "",
                comment: "Contact email for the delivery");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Orders",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "",
                comment: "Contact phone number for the delivery");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Orders");
        }
    }
}
