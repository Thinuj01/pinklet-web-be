using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class PackageDistrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Packages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "Packages");
        }
    }
}
