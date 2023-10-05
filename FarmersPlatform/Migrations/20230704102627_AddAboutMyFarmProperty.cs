using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmersPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddAboutMyFarmProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutMyFarm",
                table: "Farmers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutMyFarm",
                table: "Farmers");
        }
    }
}
