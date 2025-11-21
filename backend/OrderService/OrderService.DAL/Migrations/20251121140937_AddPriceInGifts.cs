using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceInGifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Gifts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Gifts");
        }
    }
}
