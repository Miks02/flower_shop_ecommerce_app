using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentAndPreviousPointsToLoyaltyTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_LoyaltyTransactions_Points_NonNegative",
                table: "LoyaltyTransactions");

            migrationBuilder.RenameColumn(
                name: "Points",
                table: "LoyaltyTransactions",
                newName: "PreviousPoints");

            migrationBuilder.AddColumn<int>(
                name: "CurrentPoints",
                table: "LoyaltyTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_LoyaltyTransactions_CurrentPoints_NonNegative",
                table: "LoyaltyTransactions",
                sql: "CurrentPoints >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_LoyaltyTransactions_CurrentPoints_NonNegative",
                table: "LoyaltyTransactions");

            migrationBuilder.DropColumn(
                name: "CurrentPoints",
                table: "LoyaltyTransactions");

            migrationBuilder.RenameColumn(
                name: "PreviousPoints",
                table: "LoyaltyTransactions",
                newName: "Points");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LoyaltyTransactions_Points_NonNegative",
                table: "LoyaltyTransactions",
                sql: "Points >= 0");
        }
    }
}
