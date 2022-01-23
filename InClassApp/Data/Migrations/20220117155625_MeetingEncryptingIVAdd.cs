using Microsoft.EntityFrameworkCore.Migrations;

namespace InClassApp.Data.Migrations
{
    public partial class MeetingEncryptingIVAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastlyGeneratedCodeIV",
                table: "Meetings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastlyGeneratedCodeIV",
                table: "Meetings");
        }
    }
}
