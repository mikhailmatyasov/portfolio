using Microsoft.EntityFrameworkCore.Migrations;

namespace WeSafe.Nano.DAL.Migrations
{
    public partial class Add_GlobalSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceType",
                table: "Devices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GlobalSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KeepingDeviceLogsDays = table.Column<int>(nullable: false),
                    KeepingCameraLogsDays = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalSettings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalSettings");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "Devices");
        }
    }
}
