using Microsoft.EntityFrameworkCore.Migrations;

namespace WeSafe.DAL.Migrations
{
    public partial class RemoveWeSmartDependencies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direction",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "DirectionLeft",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "DirectionRight",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "IsForRecognition",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "SeparateIndex",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "TimeStartRecord",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "TimeStopRecord",
                table: "Cameras");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Direction",
                table: "Cameras",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DirectionLeft",
                table: "Cameras",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DirectionRight",
                table: "Cameras",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsForRecognition",
                table: "Cameras",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SeparateIndex",
                table: "Cameras",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TimeStartRecord",
                table: "Cameras",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TimeStopRecord",
                table: "Cameras",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
