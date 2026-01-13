using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RefactorBouquetImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BouquetImages_BouquetId",
                table: "BouquetImages");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BouquetImage_Position",
                table: "BouquetImages");

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "BouquetImages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SizeId",
                table: "BouquetImages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BouquetImages_BouquetId_SizeId_IsMain",
                table: "BouquetImages",
                columns: new[] { "BouquetId", "SizeId", "IsMain" });

            migrationBuilder.AddForeignKey(
                name: "FK_BouquetImages_BouquetSizes_BouquetId_SizeId",
                table: "BouquetImages",
                columns: new[] { "BouquetId", "SizeId" },
                principalTable: "BouquetSizes",
                principalColumns: new[] { "BouquetId", "SizeId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BouquetImages_BouquetSizes_BouquetId_SizeId",
                table: "BouquetImages");

            migrationBuilder.DropIndex(
                name: "IX_BouquetImages_BouquetId_SizeId_IsMain",
                table: "BouquetImages");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "BouquetImages");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "BouquetImages");

            migrationBuilder.CreateIndex(
                name: "IX_BouquetImages_BouquetId",
                table: "BouquetImages",
                column: "BouquetId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BouquetImage_Position",
                table: "BouquetImages",
                sql: "\"Position\" BETWEEN 1 AND 3");
        }
    }
}
