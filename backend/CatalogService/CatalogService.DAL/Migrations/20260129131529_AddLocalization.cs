using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sizes_Name",
                table: "Sizes");

            migrationBuilder.DropIndex(
                name: "IX_Recipients_Name",
                table: "Recipients");

            migrationBuilder.DropIndex(
                name: "IX_Flowers_Name",
                table: "Flowers");

            migrationBuilder.DropIndex(
                name: "IX_Events_Name",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Bouquets_Name",
                table: "Bouquets");

            migrationBuilder.Sql("ALTER TABLE \"Sizes\" ALTER COLUMN \"Name\" TYPE jsonb USING jsonb_build_object('ua', \"Name\", 'en', \"Name\");");
            migrationBuilder.Sql("ALTER TABLE \"Recipients\" ALTER COLUMN \"Name\" TYPE jsonb USING jsonb_build_object('ua', \"Name\", 'en', \"Name\");");
            migrationBuilder.Sql("ALTER TABLE \"Flowers\" ALTER COLUMN \"Name\" TYPE jsonb USING jsonb_build_object('ua', \"Name\", 'en', \"Name\");");
            migrationBuilder.Sql("ALTER TABLE \"Events\" ALTER COLUMN \"Name\" TYPE jsonb USING jsonb_build_object('ua', \"Name\", 'en', \"Name\");");
            migrationBuilder.Sql("ALTER TABLE \"Bouquets\" ALTER COLUMN \"Name\" TYPE jsonb USING jsonb_build_object('ua', \"Name\", 'en', \"Name\");");
            migrationBuilder.Sql("ALTER TABLE \"Bouquets\" ALTER COLUMN \"Description\" TYPE jsonb USING jsonb_build_object('ua', \"Description\", 'en', \"Description\");");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sizes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Recipients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Flowers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Events",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Bouquets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Bouquets",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sizes_Name",
                table: "Sizes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_Name",
                table: "Recipients",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flowers_Name",
                table: "Flowers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_Name",
                table: "Events",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bouquets_Name",
                table: "Bouquets",
                column: "Name",
                unique: true);
        }
    }
}
