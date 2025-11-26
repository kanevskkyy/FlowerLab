using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BouquetImage",
                table: "OrderItems",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<Guid>(
                name: "SizeId",
                table: "OrderItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "SizeName",
                table: "OrderItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OrderItemFlowers",
                columns: table => new
                {
                    OrderItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    FlowerId = table.Column<Guid>(type: "uuid", nullable: false),
                    FlowerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FlowerColor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemFlowers", x => new { x.OrderItemId, x.FlowerId });
                    table.CheckConstraint("CK_OrderItemFlower_Quantity_Positive", "\"Quantity\" > 0");
                    table.ForeignKey(
                        name: "FK_OrderItemFlowers_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderItem_Count_Positive",
                table: "OrderItems",
                sql: "\"Count\" > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemFlowers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderItem_Count_Positive",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SizeName",
                table: "OrderItems");

            migrationBuilder.AlterColumn<string>(
                name: "BouquetImage",
                table: "OrderItems",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);
        }
    }
}
