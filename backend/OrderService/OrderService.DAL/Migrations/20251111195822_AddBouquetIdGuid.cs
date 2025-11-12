using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBouquetIdGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BouquetId",
                table: "OrderItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BouquetId",
                table: "OrderItems");
        }
    }
}
