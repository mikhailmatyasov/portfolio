using Microsoft.EntityFrameworkCore.Migrations;

namespace WeSafe.DAL.Migrations
{
    public partial class Update_Camera : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cameras_CameraName",
                table: "Cameras");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_CameraName",
                table: "Cameras",
                column: "CameraName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cameras_CameraName",
                table: "Cameras");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_CameraName",
                table: "Cameras",
                column: "CameraName",
                unique: true);
        }
    }
}
