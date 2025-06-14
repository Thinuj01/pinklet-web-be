using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class _3DModelScheama2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cakes3dModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Occation = table.Column<string>(type: "text", nullable: false),
                    BaseShape = table.Column<string>(type: "text", nullable: false),
                    BaseShapeSize = table.Column<string>(type: "text", nullable: false),
                    NoLayers = table.Column<int>(type: "integer", nullable: false),
                    LayerShape = table.Column<string>(type: "text", nullable: false),
                    IcingType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cakes3dModel", x => x.Id);
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
                name: "Cakes3dModel");
        }
    }
}
