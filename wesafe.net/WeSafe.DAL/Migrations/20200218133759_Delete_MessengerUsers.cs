using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WeSafe.DAL.Migrations
{
    public partial class Delete_MessengerUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""CameraSettings""");

            migrationBuilder.DropTable(
                name: "MessengerUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ClientSubscribers",
                maxLength: 100,
                nullable: false,
                defaultValue: "Unnamed",
                oldClrType: typeof(string),
                oldMaxLength: 100);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ClientSubscribers",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldDefaultValue: "Unnamed");

            migrationBuilder.CreateTable(
                name: "MessengerUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false, defaultValue: "Unnamed"),
                    Permissions = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Schedule = table.Column<string>(nullable: true),
                    Settings = table.Column<string>(nullable: true),
                    TelegramId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessengerUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessengerUsers_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessengerUsers_ClientId",
                table: "MessengerUsers",
                column: "ClientId");
        }
    }
}
