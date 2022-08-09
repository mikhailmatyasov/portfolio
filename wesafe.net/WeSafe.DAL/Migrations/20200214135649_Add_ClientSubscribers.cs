using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_ClientSubscribers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientSubscribers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientId = table.Column<int>(nullable: false),
                    Phone = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Permissions = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSubscribers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSubscribers_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelegramUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Phone = table.Column<string>(maxLength: 20, nullable: false),
                    TelegramId = table.Column<long>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Settings = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Mute = table.Column<DateTimeOffset>(nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientSubscriberSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientSubscriberId = table.Column<int>(nullable: false),
                    CameraId = table.Column<int>(nullable: false),
                    Mute = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSubscriberSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSubscriberSettings_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientSubscriberSettings_ClientSubscribers_ClientSubscriber~",
                        column: x => x.ClientSubscriberId,
                        principalTable: "ClientSubscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscribers_ClientId_Phone",
                table: "ClientSubscribers",
                columns: new[] { "ClientId", "Phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriberSettings_CameraId",
                table: "ClientSubscriberSettings",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriberSettings_ClientSubscriberId",
                table: "ClientSubscriberSettings",
                column: "ClientSubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_Phone",
                table: "TelegramUsers",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_TelegramId",
                table: "TelegramUsers",
                column: "TelegramId",
                unique: true);

            migrationBuilder.Sql(@"INSERT INTO ""ClientSubscribers"" (""ClientId"", ""Phone"", ""Name"", ""Permissions"", ""IsActive"", ""CreatedAt"") SELECT ""ClientId"", ""Phone"", ""Name"", ""Permissions"", TRUE, CURRENT_TIMESTAMP FROM ""MessengerUsers""");
            migrationBuilder.Sql(@"
INSERT INTO ""TelegramUsers"" (""Phone"", ""TelegramId"", ""Status"", ""CreatedAt"") SELECT ""Phone"", CAST(""TelegramId"" AS BIGINT), 'active', CURRENT_TIMESTAMP FROM
(SELECT DISTINCT ""Phone"", CAST(""TelegramId"" AS BIGINT) FROM ""MessengerUsers"" WHERE ""TelegramId"" IS NOT NULL) a");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientSubscriberSettings");

            migrationBuilder.DropTable(
                name: "TelegramUsers");

            migrationBuilder.DropTable(
                name: "ClientSubscribers");
        }
    }
}
