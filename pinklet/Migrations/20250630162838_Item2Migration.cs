using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class Item2Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VenderId",
                table: "Items",
                newName: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VendorId",
                table: "Items",
                newName: "VenderId");
        }
    }
}
