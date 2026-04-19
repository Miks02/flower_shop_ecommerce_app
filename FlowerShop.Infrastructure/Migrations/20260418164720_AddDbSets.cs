using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDbSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OccasionProduct_Occasion_OccasionsId",
                table: "OccasionProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_OccasionProduct_Product_ProductsId",
                table: "OccasionProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_CategoryId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFlower_Flower_FlowerId",
                table: "ProductFlower");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFlower_Product_ProductId",
                table: "ProductFlower");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductFlower",
                table: "ProductFlower");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Occasion",
                table: "Occasion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Flower",
                table: "Flower");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "ProductFlower",
                newName: "ProductFlowers");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "Occasion",
                newName: "Occasions");

            migrationBuilder.RenameTable(
                name: "Flower",
                newName: "Flowers");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_ProductFlower_FlowerId",
                table: "ProductFlowers",
                newName: "IX_ProductFlowers_FlowerId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_Price",
                table: "Products",
                newName: "IX_Products_Price");

            migrationBuilder.RenameIndex(
                name: "IX_Product_CategoryId_Price",
                table: "Products",
                newName: "IX_Products_CategoryId_Price");

            migrationBuilder.RenameIndex(
                name: "IX_Flower_Name",
                table: "Flowers",
                newName: "IX_Flowers_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Flower_Color",
                table: "Flowers",
                newName: "IX_Flowers_Color");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductFlowers",
                table: "ProductFlowers",
                columns: new[] { "ProductId", "FlowerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Occasions",
                table: "Occasions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flowers",
                table: "Flowers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OccasionProduct_Occasions_OccasionsId",
                table: "OccasionProduct",
                column: "OccasionsId",
                principalTable: "Occasions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OccasionProduct_Products_ProductsId",
                table: "OccasionProduct",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFlowers_Flowers_FlowerId",
                table: "ProductFlowers",
                column: "FlowerId",
                principalTable: "Flowers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFlowers_Products_ProductId",
                table: "ProductFlowers",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OccasionProduct_Occasions_OccasionsId",
                table: "OccasionProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_OccasionProduct_Products_ProductsId",
                table: "OccasionProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFlowers_Flowers_FlowerId",
                table: "ProductFlowers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFlowers_Products_ProductId",
                table: "ProductFlowers");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductFlowers",
                table: "ProductFlowers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Occasions",
                table: "Occasions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Flowers",
                table: "Flowers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "ProductFlowers",
                newName: "ProductFlower");

            migrationBuilder.RenameTable(
                name: "Occasions",
                newName: "Occasion");

            migrationBuilder.RenameTable(
                name: "Flowers",
                newName: "Flower");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_Products_Price",
                table: "Product",
                newName: "IX_Product_Price");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId_Price",
                table: "Product",
                newName: "IX_Product_CategoryId_Price");

            migrationBuilder.RenameIndex(
                name: "IX_ProductFlowers_FlowerId",
                table: "ProductFlower",
                newName: "IX_ProductFlower_FlowerId");

            migrationBuilder.RenameIndex(
                name: "IX_Flowers_Name",
                table: "Flower",
                newName: "IX_Flower_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Flowers_Color",
                table: "Flower",
                newName: "IX_Flower_Color");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductFlower",
                table: "ProductFlower",
                columns: new[] { "ProductId", "FlowerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Occasion",
                table: "Occasion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flower",
                table: "Flower",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OccasionProduct_Occasion_OccasionsId",
                table: "OccasionProduct",
                column: "OccasionsId",
                principalTable: "Occasion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OccasionProduct_Product_ProductsId",
                table: "OccasionProduct",
                column: "ProductsId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_CategoryId",
                table: "Product",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFlower_Flower_FlowerId",
                table: "ProductFlower",
                column: "FlowerId",
                principalTable: "Flower",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFlower_Product_ProductId",
                table: "ProductFlower",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
