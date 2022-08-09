using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_Device_Indicators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceIndicators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<int>(nullable: false),
                    CpuUtilization = table.Column<double>(nullable: true),
                    GpuUtilization = table.Column<double>(nullable: true),
                    MemoryUtilization = table.Column<double>(nullable: true),
                    Temperature = table.Column<double>(nullable: true),
                    CamerasFps = table.Column<string>(nullable: true),
                    Traffic = table.Column<double>(nullable: true),
                    Time = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceIndicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceIndicators_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceIndicators_DeviceId",
                table: "DeviceIndicators",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceIndicators");
        }
    }
}
