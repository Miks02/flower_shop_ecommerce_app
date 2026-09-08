using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDelivererRelationWithReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Deliverers_DelivererId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_DelivererId_OrderId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "DelivererId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OrderId_ReviewerId",
                table: "Reviews",
                columns: new[] { "OrderId", "ReviewerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_OrderId_ReviewerId",
                table: "Reviews");

            migrationBuilder.AddColumn<string>(
                name: "DelivererId",
                table: "Reviews",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DelivererId_OrderId",
                table: "Reviews",
                columns: new[] { "DelivererId", "OrderId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Deliverers_DelivererId",
                table: "Reviews",
                column: "DelivererId",
                principalTable: "Deliverers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
