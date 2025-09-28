using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class RatingNoMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RatingNo",
                table: "Items",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingNo",
                table: "Items");
        }
    }
}
