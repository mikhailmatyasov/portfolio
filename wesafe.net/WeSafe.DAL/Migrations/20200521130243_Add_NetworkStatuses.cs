using Microsoft.EntityFrameworkCore.Migrations;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_NetworkStatuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NetworkStatus",
                table: "Devices",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetworkStatus",
                table: "Cameras",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetworkStatus",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "NetworkStatus",
                table: "Cameras");
        }
    }
}
