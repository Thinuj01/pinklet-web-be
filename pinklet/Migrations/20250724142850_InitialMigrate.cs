using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cakes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CakeCode = table.Column<string>(type: "text", nullable: false),
                    CakeName = table.Column<string>(type: "text", nullable: false),
                    CakeCategory = table.Column<string>(type: "text", nullable: false),
                    CakeTags = table.Column<string>(type: "text", nullable: false),
                    CakePrice = table.Column<double>(type: "double precision", nullable: false),
                    CakeRating = table.Column<int>(type: "integer", nullable: false),
                    CakeDescription = table.Column<string>(type: "text", nullable: false),
                    CakeImageLink1 = table.Column<string>(type: "text", nullable: true),
                    CakeImageLink2 = table.Column<string>(type: "text", nullable: true),
                    CakeImageLink3 = table.Column<string>(type: "text", nullable: true),
                    CakeImageLink4 = table.Column<string>(type: "text", nullable: true),
                    Cake3dModelLink = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cakes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cakes3dModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CakeCode = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Occation = table.Column<string>(type: "text", nullable: false),
                    BaseShape = table.Column<string>(type: "text", nullable: false),
                    BaseShapeSize = table.Column<string>(type: "text", nullable: false),
                    NoLayers = table.Column<int>(type: "integer", nullable: false),
                    LayerShape = table.Column<string>(type: "text", nullable: false),
                    IcingType = table.Column<string>(type: "text", nullable: false),
                    Toppers = table.Column<string>(type: "text", nullable: true),
                    IsReqested = table.Column<bool>(type: "boolean", nullable: true),
                    RequestedPrice = table.Column<double>(type: "double precision", nullable: true),
                    RequestedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cakes3dModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Availability = table.Column<string>(type: "text", nullable: false),
                    EmailVerificationToken = table.Column<string>(type: "text", nullable: true),
                    TokenGeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProfileImageLink = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CakeLayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CakeId = table.Column<int>(type: "integer", nullable: false),
                    LayerNo = table.Column<int>(type: "integer", nullable: false),
                    LayerFlavor = table.Column<string>(type: "text", nullable: false),
                    LayerHeight = table.Column<string>(type: "text", nullable: false),
                    LayerColorizeType = table.Column<string>(type: "text", nullable: false),
                    LayerSoidColor = table.Column<string>(type: "text", nullable: false),
                    LayerGradientColor1 = table.Column<string>(type: "text", nullable: false),
                    LayerGradientColor2 = table.Column<string>(type: "text", nullable: false),
                    LayerGradientDirection = table.Column<string>(type: "text", nullable: false),
                    LayerPatternType = table.Column<string>(type: "text", nullable: false),
                    LayerPatternColor = table.Column<string>(type: "text", nullable: false),
                    LayerPatternBGColor = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VendorId = table.Column<int>(type: "integer", nullable: false),
                    ItemCode = table.Column<string>(type: "text", nullable: false),
                    ItemName = table.Column<string>(type: "text", nullable: false),
                    ItemCategory = table.Column<string>(type: "text", nullable: false),
                    ItemSubCategory = table.Column<string>(type: "text", nullable: false),
                    ItemTags = table.Column<string>(type: "text", nullable: false),
                    ItemStock = table.Column<int>(type: "integer", nullable: false),
                    ItemPrice = table.Column<double>(type: "double precision", nullable: false),
                    ItemRating = table.Column<int>(type: "integer", nullable: false),
                    ItemDescription = table.Column<string>(type: "text", nullable: false),
                    ItemImageLink1 = table.Column<string>(type: "text", nullable: true),
                    ItemImageLink2 = table.Column<string>(type: "text", nullable: true),
                    ItemImageLink3 = table.Column<string>(type: "text", nullable: true),
                    ItemImageLink4 = table.Column<string>(type: "text", nullable: true),
                    ItemImageLink5 = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PackageCode = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CakeId = table.Column<int>(type: "integer", nullable: true),
                    ThreeDCakeId = table.Column<int>(type: "integer", nullable: true),
                    _3DCakeModelId = table.Column<int>(type: "integer", nullable: true)
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
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ShopName = table.Column<string>(type: "text", nullable: false),
                    ShopDistrict = table.Column<string>(type: "text", nullable: false),
                    ShopCity = table.Column<string>(type: "text", nullable: false),
                    ShopDescription = table.Column<string>(type: "text", nullable: false),
                    ShopProfileImageLink = table.Column<string>(type: "text", nullable: true),
                    ShopCoverImageLink = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    IDNumber = table.Column<string>(type: "text", nullable: false),
                    IDImageLink1 = table.Column<string>(type: "text", nullable: false),
                    IDImageLink2 = table.Column<string>(type: "text", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PackageId = table.Column<int>(type: "integer", nullable: false),
                    IsCheckedOut = table.Column<bool>(type: "boolean", nullable: false)
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
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    PackageId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_UserId",
                table: "Vendors",
                column: "UserId",
                unique: true);
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
                name: "Vendors");

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
