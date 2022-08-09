using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_Rtsp_Paths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CameraManufactors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Manufactor = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraManufactors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CameraMarks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CameraManufactorId = table.Column<int>(nullable: false),
                    Model = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraMarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CameraMarks_CameraManufactors_CameraManufactorId",
                        column: x => x.CameraManufactorId,
                        principalTable: "CameraManufactors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RtspPaths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Path = table.Column<string>(nullable: true),
                    CameraMarkId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RtspPaths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RtspPaths_CameraMarks_CameraMarkId",
                        column: x => x.CameraMarkId,
                        principalTable: "CameraMarks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CameraMarks_CameraManufactorId",
                table: "CameraMarks",
                column: "CameraManufactorId");

            migrationBuilder.CreateIndex(
                name: "IX_RtspPaths_CameraMarkId",
                table: "RtspPaths",
                column: "CameraMarkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RtspPaths");

            migrationBuilder.DropTable(
                name: "CameraMarks");

            migrationBuilder.DropTable(
                name: "CameraManufactors");
        }
    }
}
