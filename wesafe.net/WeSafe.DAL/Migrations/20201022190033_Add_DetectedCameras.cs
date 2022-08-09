using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_DetectedCameras : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetectedCameras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Ip = table.Column<string>(maxLength: 15, nullable: false),
                    Port = table.Column<string>(maxLength: 10, nullable: false),
                    Login = table.Column<string>(maxLength: 100, nullable: true),
                    Password = table.Column<string>(maxLength: 50, nullable: true),
                    DeviceId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    DetectingMethod = table.Column<string>(nullable: true),
                    ConnectFailureText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetectedCameras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetectedCameras_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetectedCameras_DeviceId",
                table: "DetectedCameras",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetectedCameras");
        }
    }
}
