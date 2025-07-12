using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class CakeModelUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReqested",
                table: "Cakes3dModel",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RequestedPrice",
                table: "Cakes3dModel",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReqested",
                table: "Cakes3dModel");

            migrationBuilder.DropColumn(
                name: "RequestedPrice",
                table: "Cakes3dModel");
        }
    }
}
