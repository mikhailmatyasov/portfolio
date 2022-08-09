using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_Device_SshPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentSshPassword",
                table: "Devices",
                nullable: false,
                defaultValue: "44JxTOaKbCyO0v5E/0vzFJn0xfdwnFJzdVWkrVOPHMs=");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateDatePassword",
                table: "Devices",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'");

            migrationBuilder.AddColumn<string>(
                name: "PreviousSshPassword",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSshPassword",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "LastUpdateDatePassword",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "PreviousSshPassword",
                table: "Devices");
        }
    }
}
