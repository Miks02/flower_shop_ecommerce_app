using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetRelationshipBetweenDelivererAndOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DelivererId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DelivererId",
                table: "Orders",
                column: "DelivererId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Deliverers_DelivererId",
                table: "Orders",
                column: "DelivererId",
                principalTable: "Deliverers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Deliverers_DelivererId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DelivererId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DelivererId",
                table: "Orders");
        }
    }
}
