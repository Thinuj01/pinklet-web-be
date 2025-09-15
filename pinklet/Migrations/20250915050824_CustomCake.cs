using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pinklet.Migrations
{
    /// <inheritdoc />
    public partial class CustomCake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomCakes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CakeCode = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CakeWeight = table.Column<string>(type: "text", nullable: false),
                    CakePrice = table.Column<double>(type: "double precision", nullable: true),
                    CakeImageLink1 = table.Column<string>(type: "text", nullable: true),
                    CakeImageLink2 = table.Column<string>(type: "text", nullable: true),
                    CakeImageLink3 = table.Column<string>(type: "text", nullable: true),
                    CakeImageLink4 = table.Column<string>(type: "text", nullable: true),
                    CakeImageLink5 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomCakes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomCakes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomCakes_UserId",
                table: "CustomCakes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomCakes");
        }
    }
}
