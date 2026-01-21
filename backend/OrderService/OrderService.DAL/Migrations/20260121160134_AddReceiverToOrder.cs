using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiverToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "DeliveryInformations");

            migrationBuilder.DropColumn(
                name: "ReceiverPhone",
                table: "DeliveryInformations");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverPhone",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReceiverPhone",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "DeliveryInformations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverPhone",
                table: "DeliveryInformations",
                type: "text",
                nullable: true);
        }
    }
}
