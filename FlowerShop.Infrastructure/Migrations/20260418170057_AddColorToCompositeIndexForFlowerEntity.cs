using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColorToCompositeIndexForFlowerEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flowers_Name_FlowerCategory",
                table: "Flowers");

            migrationBuilder.CreateIndex(
                name: "IX_Flowers_Name_FlowerCategory_Color",
                table: "Flowers",
                columns: new[] { "Name", "FlowerCategory", "Color" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flowers_Name_FlowerCategory_Color",
                table: "Flowers");

            migrationBuilder.CreateIndex(
                name: "IX_Flowers_Name_FlowerCategory",
                table: "Flowers",
                columns: new[] { "Name", "FlowerCategory" },
                unique: true);
        }
    }
}
