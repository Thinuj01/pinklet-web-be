using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class OrderItemAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemPackages",
                table: "ItemPackages");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ItemPackages",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemPackages",
                table: "ItemPackages",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ItemPackageId = table.Column<int>(type: "integer", nullable: false),
                    VendorId = table.Column<int>(type: "integer", nullable: false),
                    Progress = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemPackages_ItemId",
                table: "ItemPackages",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ItemPackageId",
                table: "OrderItems",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_VendorId",
                table: "OrderItems",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemPackages",
                table: "ItemPackages");

            migrationBuilder.DropIndex(
                name: "IX_ItemPackages_ItemId",
                table: "ItemPackages");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ItemPackages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemPackages",
                table: "ItemPackages",
                columns: new[] { "ItemId", "PackageId" });
        }
    }
}
