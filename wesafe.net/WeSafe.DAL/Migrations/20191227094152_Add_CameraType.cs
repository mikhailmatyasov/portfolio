using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_CameraType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CameraTypeId",
                table: "Cameras",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CameraVendors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraVendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CameraTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    RtspTemplate = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CameraVendorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CameraTypes_CameraVendors_CameraVendorId",
                        column: x => x.CameraVendorId,
                        principalTable: "CameraVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_CameraTypeId",
                table: "Cameras",
                column: "CameraTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CameraTypes_CameraVendorId",
                table: "CameraTypes",
                column: "CameraVendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cameras_CameraTypes_CameraTypeId",
                table: "Cameras",
                column: "CameraTypeId",
                principalTable: "CameraTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cameras_CameraTypes_CameraTypeId",
                table: "Cameras");

            migrationBuilder.DropTable(
                name: "CameraTypes");

            migrationBuilder.DropTable(
                name: "CameraVendors");

            migrationBuilder.DropIndex(
                name: "IX_Cameras_CameraTypeId",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "CameraTypeId",
                table: "Cameras");
        }
    }
}
