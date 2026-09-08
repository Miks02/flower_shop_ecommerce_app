using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameReviewToServiceReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReviewId",
                table: "Orders",
                newName: "ServiceReviewId");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "ServiceReviews");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_OrderId",
                table: "ServiceReviews",
                newName: "IX_ServiceReviews_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_OrderId_ReviewerId",
                table: "ServiceReviews",
                newName: "IX_ServiceReviews_OrderId_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_ReviewerId",
                table: "ServiceReviews",
                newName: "IX_ServiceReviews_ReviewerId");

            migrationBuilder.Sql("EXEC sp_rename 'PK_Reviews', 'PK_ServiceReviews', 'OBJECT';");
            migrationBuilder.Sql("EXEC sp_rename 'FK_Reviews_Orders_OrderId', 'FK_ServiceReviews_Orders_OrderId', 'OBJECT';");
            migrationBuilder.Sql("EXEC sp_rename 'FK_Reviews_Users_ReviewerId', 'FK_ServiceReviews_Users_ReviewerId', 'OBJECT';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("EXEC sp_rename 'FK_ServiceReviews_Orders_OrderId', 'FK_Reviews_Orders_OrderId', 'OBJECT';");
            migrationBuilder.Sql("EXEC sp_rename 'FK_ServiceReviews_Users_ReviewerId', 'FK_Reviews_Users_ReviewerId', 'OBJECT';");
            migrationBuilder.Sql("EXEC sp_rename 'PK_ServiceReviews', 'PK_Reviews', 'OBJECT';");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceReviews_OrderId",
                table: "ServiceReviews",
                newName: "IX_Reviews_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceReviews_OrderId_ReviewerId",
                table: "ServiceReviews",
                newName: "IX_Reviews_OrderId_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceReviews_ReviewerId",
                table: "ServiceReviews",
                newName: "IX_Reviews_ReviewerId");

            migrationBuilder.RenameTable(
                name: "ServiceReviews",
                newName: "Reviews");

            migrationBuilder.RenameColumn(
                name: "ServiceReviewId",
                table: "Orders",
                newName: "ReviewId");
        }
    }
}
