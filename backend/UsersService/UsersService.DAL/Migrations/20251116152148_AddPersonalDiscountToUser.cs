using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalDiscountToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonalDiscountPercentage",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalDiscountPercentage",
                table: "AspNetUsers");
        }
    }
}
