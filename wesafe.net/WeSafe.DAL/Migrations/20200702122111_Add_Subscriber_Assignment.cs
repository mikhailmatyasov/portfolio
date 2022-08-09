using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_Subscriber_Assignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientSubscriberAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientSubscriberId = table.Column<int>(nullable: false),
                    DeviceId = table.Column<int>(nullable: false),
                    CameraId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSubscriberAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSubscriberAssignments_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientSubscriberAssignments_ClientSubscribers_ClientSubscri~",
                        column: x => x.ClientSubscriberId,
                        principalTable: "ClientSubscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientSubscriberAssignments_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriberAssignments_CameraId",
                table: "ClientSubscriberAssignments",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriberAssignments_ClientSubscriberId",
                table: "ClientSubscriberAssignments",
                column: "ClientSubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriberAssignments_DeviceId",
                table: "ClientSubscriberAssignments",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientSubscriberAssignments");
        }
    }
}
