using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlowerShop.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataForCategoriesAndOccasions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Buketi" },
                    { 2, "Aranžmani" },
                    { 3, "Specijalni pokloni" },
                    { 4, "101 Ruža" },
                    { 5, "Saksijsko cveće" },
                    { 6, "Box mede" },
                    { 7, "Dehidrirane ruže" },
                    { 8, "Venci i suze" }
                });

            migrationBuilder.InsertData(
                table: "Occasions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Dan zaljubljenih" },
                    { 2, "8. Mart - Dan žena" },
                    { 3, "Rodjendan" },
                    { 4, "Svadba i venčanje" }
                });

            migrationBuilder.CreateIndex(
                name: "UX_Categorys_Name",
                table: "Categories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Categorys_Name",
                table: "Categories");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Occasions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Occasions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Occasions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Occasions",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
