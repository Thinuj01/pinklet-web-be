using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class CustomPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomCakeId",
                table: "Packages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CustomCakeId",
                table: "Packages",
                column: "CustomCakeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_CustomCakes_CustomCakeId",
                table: "Packages",
                column: "CustomCakeId",
                principalTable: "CustomCakes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_CustomCakes_CustomCakeId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_CustomCakeId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "CustomCakeId",
                table: "Packages");
        }
    }
}
