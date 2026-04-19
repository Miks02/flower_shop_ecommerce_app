using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataForFlowersAndUniqueCompositeIndeksOnNameAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Flowers",
                columns: new[] { "Id", "Color", "FlowerCategory", "Name", "Stock" },
                values: new object[,]
                {
                    { 1, "Red", 0, "Ruža", 100 },
                    { 2, "Yellow", 1, "Lala", 150 },
                    { 3, "White", 2, "Ljiljan", 80 },
                    { 4, "Pink", 0, "Bela rada", 120 },
                    { 5, "Red", 0, "Karanfil", 200 },
                    { 6, "Orange", 0, "Gerber", 90 },
                    { 7, "Purple", 0, "Orhideja", 50 },
                    { 8, "Blue", 0, "Iris", 70 },
                    { 9, "Yellow", 0, "Narcis", 110 },
                    { 10, "Yellow", 0, "Suncokret", 60 },
                    { 11, "Purple", 0, "Jorgovan", 40 },
                    { 12, "White", 0, "Magnolija", 30 },
                    { 13, "Yellow", 0, "Hrizantema", 100 },
                    { 14, "Pink", 0, "Božur", 45 },
                    { 15, "White", 0, "Jasmin", 85 },
                    { 16, "Purple", 1, "Lavanda", 300 },
                    { 17, "Yellow", 0, "Mimoza", 55 },
                    { 18, "White", 1, "Ruža", 40 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flowers_Name_FlowerCategory",
                table: "Flowers",
                columns: new[] { "Name", "FlowerCategory" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flowers_Name_FlowerCategory",
                table: "Flowers");

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Flowers",
                keyColumn: "Id",
                keyValue: 18);
        }
    }
}
