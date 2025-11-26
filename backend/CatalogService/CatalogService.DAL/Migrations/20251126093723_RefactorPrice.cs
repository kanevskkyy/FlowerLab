using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RefactorPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Bouquets");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "BouquetSizes",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "BouquetSizes");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Bouquets",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
