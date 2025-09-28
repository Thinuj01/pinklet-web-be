using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class PackageCartMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_PackageId",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PackageId",
                table: "Carts",
                column: "PackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_PackageId",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PackageId",
                table: "Carts",
                column: "PackageId",
                unique: true);
        }
    }
}
