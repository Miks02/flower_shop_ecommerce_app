using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAndItsRelatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlowerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FlowerCategory = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowerTypes", x => x.Id);
                    table.CheckConstraint("CK_FlowerTypes_Stock_Positive", "Stock >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Occasions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Occasions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(750)", maxLength: 750, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(750)", maxLength: 750, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PromoPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Stock = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.CheckConstraint("CK_Products_Price_Positive", "Price > 0");
                    table.CheckConstraint("CK_Products_Stock_Positive", "Stock >= 0");
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OccasionProduct",
                columns: table => new
                {
                    OccasionsId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OccasionProduct", x => new { x.OccasionsId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_OccasionProduct_Occasions_OccasionsId",
                        column: x => x.OccasionsId,
                        principalTable: "Occasions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OccasionProduct_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductFlowers",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    FlowerId = table.Column<int>(type: "int", nullable: false),
                    FlowerTypeId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFlowers", x => new { x.ProductId, x.FlowerId });
                    table.CheckConstraint("CK_ProductFlowers_Quantity_Positive", "Quantity >= 0");
                    table.ForeignKey(
                        name: "FK_ProductFlowers_FlowerTypes_FlowerTypeId",
                        column: x => x.FlowerTypeId,
                        principalTable: "FlowerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductFlowers_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlowerTypes_Color",
                table: "FlowerTypes",
                column: "Color");

            migrationBuilder.CreateIndex(
                name: "IX_FlowerTypes_Name",
                table: "FlowerTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "UX_FlowerTypes_Name_Color",
                table: "FlowerTypes",
                columns: new[] { "Name", "Color" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OccasionProduct_ProductsId",
                table: "OccasionProduct",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "UX_Occasions_Name",
                table: "Occasions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductFlowers_FlowerTypeId",
                table: "ProductFlowers",
                column: "FlowerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId_Price",
                table: "Products",
                columns: new[] { "CategoryId", "Price" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Price",
                table: "Products",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "UX_Products_Name_CategoryId",
                table: "Products",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OccasionProduct");

            migrationBuilder.DropTable(
                name: "ProductFlowers");

            migrationBuilder.DropTable(
                name: "Occasions");

            migrationBuilder.DropTable(
                name: "FlowerTypes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
