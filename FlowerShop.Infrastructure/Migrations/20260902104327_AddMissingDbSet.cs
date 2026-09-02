using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliverer_Users_Id",
                table: "Deliverer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deliverer",
                table: "Deliverer");

            migrationBuilder.RenameTable(
                name: "Deliverer",
                newName: "Deliverers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deliverers",
                table: "Deliverers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliverers_Users_Id",
                table: "Deliverers",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliverers_Users_Id",
                table: "Deliverers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deliverers",
                table: "Deliverers");

            migrationBuilder.RenameTable(
                name: "Deliverers",
                newName: "Deliverer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deliverer",
                table: "Deliverer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliverer_Users_Id",
                table: "Deliverer",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
