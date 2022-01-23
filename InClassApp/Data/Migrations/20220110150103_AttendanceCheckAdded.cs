using Microsoft.EntityFrameworkCore.Migrations;

namespace InClassApp.Data.Migrations
{
    public partial class AttendanceCheckAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAttendanceCheckLaunched",
                table: "Meetings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastlyGeneratedCheckCode",
                table: "Meetings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAttendanceCheckLaunched",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "LastlyGeneratedCheckCode",
                table: "Meetings");
        }
    }
}
