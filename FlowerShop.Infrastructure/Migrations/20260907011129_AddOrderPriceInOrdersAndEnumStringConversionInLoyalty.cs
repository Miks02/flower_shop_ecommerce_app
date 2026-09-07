using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderPriceInOrdersAndEnumStringConversionInLoyalty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OrderPrice",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "LoyaltyTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderPrice",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionType",
                table: "LoyaltyTransactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
