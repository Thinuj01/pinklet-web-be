using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class InitialUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cakes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CakeCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CakeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CakeCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CakeTags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CakePrice = table.Column<double>(type: "float", nullable: false),
                    CakeRating = table.Column<int>(type: "int", nullable: false),
                    CakeDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CakeImageLink1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CakeImageLink2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CakeImageLink3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CakeImageLink4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cake3dModelLink = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cakes", x => x.Id);
                });

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
                    IcingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Toppers = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemTags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemPrice = table.Column<double>(type: "float", nullable: false),
                    ItemRating = table.Column<int>(type: "int", nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemImageLink1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemImageLink2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemImageLink3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemImageLink4 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Users_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CakeId = table.Column<int>(type: "int", nullable: false),
                    ThreeDCakeId = table.Column<int>(type: "int", nullable: false),
                    _3DCakeModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Cakes3dModel_ThreeDCakeId",
                        column: x => x.ThreeDCakeId,
                        principalTable: "Cakes3dModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Packages_Cakes3dModel__3DCakeModelId",
                        column: x => x._3DCakeModelId,
                        principalTable: "Cakes3dModel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Packages_Cakes_CakeId",
                        column: x => x.CakeId,
                        principalTable: "Cakes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Packages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    IsCheckedOut = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemPackages",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPackages", x => new { x.ItemId, x.PackageId });
                    table.ForeignKey(
                        name: "FK_ItemPackages_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemPackages_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CakeLayers_CakeId",
                table: "CakeLayers",
                column: "CakeId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PackageId",
                table: "Carts",
                column: "PackageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPackages_PackageId",
                table: "ItemPackages",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_VendorId",
                table: "Items",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages__3DCakeModelId",
                table: "Packages",
                column: "_3DCakeModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CakeId",
                table: "Packages",
                column: "CakeId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ThreeDCakeId",
                table: "Packages",
                column: "ThreeDCakeId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_UserId",
                table: "Packages",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CakeLayers");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "ItemPackages");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Cakes3dModel");

            migrationBuilder.DropTable(
                name: "Cakes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
