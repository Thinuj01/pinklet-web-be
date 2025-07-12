using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class CakeModelCakeCodeUpdated2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RequestedDate",
                table: "Cakes3dModel",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "Cakes3dModel");
        }
    }
}
