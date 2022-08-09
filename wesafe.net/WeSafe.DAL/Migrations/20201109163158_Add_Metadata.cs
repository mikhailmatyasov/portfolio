using Microsoft.EntityFrameworkCore.Migrations;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_Metadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Cameras",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Cameras");
        }
    }
}
