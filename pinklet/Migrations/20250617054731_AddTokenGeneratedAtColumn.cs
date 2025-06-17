using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenGeneratedAtColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cakes3dModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Occation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseShape = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseShapeSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoLayers = table.Column<int>(type: "int", nullable: false),
                    LayerShape = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IcingType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cakes3dModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Availability = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailVerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenGeneratedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CakeLayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CakeId = table.Column<int>(type: "int", nullable: false),
                    LayerNo = table.Column<int>(type: "int", nullable: false),
                    LayerFlavor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LayerHeight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LayerColorizeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LayerSoidColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LayerGradientColor1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LayerGradientColor2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LayerGradientDirection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LayerPatternType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LayerPatternColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LayerPatternBGColor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CakeLayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CakeLayers_Cakes3dModel_CakeId",
                        column: x => x.CakeId,
                        principalTable: "Cakes3dModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CakeLayers_CakeId",
                table: "CakeLayers",
                column: "CakeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CakeLayers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Cakes3dModel");
        }
    }
}
