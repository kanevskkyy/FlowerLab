using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameSomeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderReservations_FlowerId_IsActive",
                table: "OrderReservations");

            migrationBuilder.RenameColumn(
                name: "FlowerName",
                table: "OrderReservations",
                newName: "BouquetName");

            migrationBuilder.RenameColumn(
                name: "FlowerId",
                table: "OrderReservations",
                newName: "SizeId");

            migrationBuilder.AddColumn<Guid>(
                name: "BouquetId",
                table: "OrderReservations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "SizeName",
                table: "OrderReservations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrderReservations_BouquetId_SizeId_IsActive",
                table: "OrderReservations",
                columns: new[] { "BouquetId", "SizeId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderReservations_BouquetId_SizeId_IsActive",
                table: "OrderReservations");

            migrationBuilder.DropColumn(
                name: "BouquetId",
                table: "OrderReservations");

            migrationBuilder.DropColumn(
                name: "SizeName",
                table: "OrderReservations");

            migrationBuilder.RenameColumn(
                name: "SizeId",
                table: "OrderReservations",
                newName: "FlowerId");

            migrationBuilder.RenameColumn(
                name: "BouquetName",
                table: "OrderReservations",
                newName: "FlowerName");

            migrationBuilder.CreateIndex(
                name: "IX_OrderReservations_FlowerId_IsActive",
                table: "OrderReservations",
                columns: new[] { "FlowerId", "IsActive" });
        }
    }
}
