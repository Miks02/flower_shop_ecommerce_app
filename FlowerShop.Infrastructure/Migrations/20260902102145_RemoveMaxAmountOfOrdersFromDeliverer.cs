using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMaxAmountOfOrdersFromDeliverer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Deliverers_MaxAmountOfOrders_Positive",
                table: "Deliverer");

            migrationBuilder.DropColumn(
                name: "MaxAmountOfOrders",
                table: "Deliverer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxAmountOfOrders",
                table: "Deliverer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Deliverers_MaxAmountOfOrders_Positive",
                table: "Deliverer",
                sql: "\"MaxAmountOfOrders\" >= 0");
        }
    }
}
