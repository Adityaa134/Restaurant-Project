using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDishCreatedAtAndDishIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dishes_CategoryId",
                table: "Dishes");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Dishes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_CategoryId_CreatedAt_DishId",
                table: "Dishes",
                columns: new[] { "CategoryId", "CreatedAt", "DishId" });

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_CreatedAt_DishId",
                table: "Dishes",
                columns: new[] { "CreatedAt", "DishId" });

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_DishName",
                table: "Dishes",
                column: "DishName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dishes_CategoryId_CreatedAt_DishId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_CreatedAt_DishId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_DishName",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Dishes");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_CategoryId",
                table: "Dishes",
                column: "CategoryId");
        }
    }
}
