using Microsoft.EntityFrameworkCore.Migrations;

namespace WeSafe.DAL.Migrations
{
    public partial class UpdateCameraLog_Message : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("Message", "CameraLogs", "Parameters");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "CameraLogs",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE ""CameraLogs"" l
SET ""Message""='Alert from Camera ' || c.""CameraName"" || '!' || CASE WHEN l.""Alert"" IS TRUE THEN ' Object detected!' ELSE '' END
FROM ""Cameras"" c
WHERE ""Message"" IS NULL AND l.""CameraId""=c.""Id""
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "CameraLogs");

            migrationBuilder.RenameColumn("Parameters", "CameraLogs", "Message");
        }
    }
}
