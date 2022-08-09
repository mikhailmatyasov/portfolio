using Microsoft.EntityFrameworkCore.Migrations;

namespace WeSafe.DAL.Migrations
{
    public partial class Add_Demo_Account_Flag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Demo",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Demo",
                table: "AspNetUsers");
        }
    }
}
