using Microsoft.EntityFrameworkCore.Migrations;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_Device_MaxActiveCameras : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxActiveCameras",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxActiveCameras",
                table: "Devices");
        }
    }
}
