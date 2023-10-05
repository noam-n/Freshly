using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmersPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddTheBoolFulfilledPropertyToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "fulfilled",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FarmerId",
                table: "Orders",
                column: "FarmerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Farmers_FarmerId",
                table: "Orders",
                column: "FarmerId",
                principalTable: "Farmers",
                principalColumn: "FarmerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Farmers_FarmerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_FarmerId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "fulfilled",
                table: "Orders");
        }
    }
}
