using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalizationOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Gifts_Name",
                table: "Gifts");

            migrationBuilder.Sql("ALTER TABLE \"Gifts\" ALTER COLUMN \"Name\" TYPE jsonb USING jsonb_build_object('ua', \"Name\", 'en', \"Name\");");

            migrationBuilder.AddColumn<Dictionary<string, string>>(
                name: "Description",
                table: "Gifts",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Gifts");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Gifts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb");

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_Name",
                table: "Gifts",
                column: "Name",
                unique: true);
        }
    }
}
