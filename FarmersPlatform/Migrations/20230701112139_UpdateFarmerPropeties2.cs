using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmersPlatform.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFarmerPropeties2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Farmers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Farmers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Farmers");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Farmers");
        }
    }
}
