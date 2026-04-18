using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameFlowerTypeToFlower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFlower_FlowerType_FlowerTypeId",
                table: "ProductFlower");

            migrationBuilder.DropTable(
                name: "FlowerType");

            migrationBuilder.DropIndex(
                name: "IX_ProductFlower_FlowerTypeId",
                table: "ProductFlower");

            migrationBuilder.DropColumn(
                name: "FlowerTypeId",
                table: "ProductFlower");

            migrationBuilder.CreateTable(
                name: "Flower",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Color = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FlowerCategory = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flower", x => x.Id);
                    table.CheckConstraint("CK_Flowers_Stock_Positive", "Stock >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductFlower_FlowerId",
                table: "ProductFlower",
                column: "FlowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Flower_Color",
                table: "Flower",
                column: "Color");

            migrationBuilder.CreateIndex(
                name: "IX_Flower_Name",
                table: "Flower",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "UX_Flowers_Name_Color",
                table: "Flower",
                columns: new[] { "Name", "Color" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFlower_Flower_FlowerId",
                table: "ProductFlower",
                column: "FlowerId",
                principalTable: "Flower",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFlower_Flower_FlowerId",
                table: "ProductFlower");

            migrationBuilder.DropTable(
                name: "Flower");

            migrationBuilder.DropIndex(
                name: "IX_ProductFlower_FlowerId",
                table: "ProductFlower");

            migrationBuilder.AddColumn<int>(
                name: "FlowerTypeId",
                table: "ProductFlower",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FlowerType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Color = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FlowerCategory = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowerType", x => x.Id);
                    table.CheckConstraint("CK_FlowerTypes_Stock_Positive", "Stock >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductFlower_FlowerTypeId",
                table: "ProductFlower",
                column: "FlowerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowerType_Color",
                table: "FlowerType",
                column: "Color");

            migrationBuilder.CreateIndex(
                name: "IX_FlowerType_Name",
                table: "FlowerType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "UX_FlowerTypes_Name_Color",
                table: "FlowerType",
                columns: new[] { "Name", "Color" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFlower_FlowerType_FlowerTypeId",
                table: "ProductFlower",
                column: "FlowerTypeId",
                principalTable: "FlowerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
