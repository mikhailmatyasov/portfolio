using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_CameraLogEntries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CameraLogEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CameraLogId = table.Column<int>(nullable: false),
                    TypeKey = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CameraLogEntries_CameraLogs_CameraLogId",
                        column: x => x.CameraLogId,
                        principalTable: "CameraLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CameraLogEntries_CameraLogId",
                table: "CameraLogEntries",
                column: "CameraLogId");

            migrationBuilder.Sql(
                "update \"Cameras\" set \"LastImagePath\"=REPLACE(\"LastImagePath\", 'app/records', 'https://40.67.203.168/files')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CameraLogEntries");
        }
    }
}
