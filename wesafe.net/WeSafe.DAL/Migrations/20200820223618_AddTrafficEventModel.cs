using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WeSafe.DAL.Migrations
{
    public partial class AddTrafficEventModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdateDatePassword",
                table: "Devices",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "now() at time zone 'utc'");

            migrationBuilder.CreateTable(
                name: "TrafficEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    DeviceMAC = table.Column<string>(nullable: true),
                    CameraId = table.Column<int>(nullable: false),
                    UtcDateTime = table.Column<DateTime>(nullable: false),
                    Direction = table.Column<int>(nullable: false),
                    ObjectId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrafficEvents_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrafficEvents_CameraId",
                table: "TrafficEvents",
                column: "CameraId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrafficEvents");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdateDatePassword",
                table: "Devices",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'",
                oldClrType: typeof(DateTime));
        }
    }
}
