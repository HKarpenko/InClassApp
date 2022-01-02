using Microsoft.EntityFrameworkCore.Migrations;

namespace InClassApp.Data.Migrations
{
    public partial class StudentRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "PresenceRecords",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    Index = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Student_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentGroupRelation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(nullable: false),
                    StudentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroupRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentGroupRelation_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGroupRelation_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PresenceRecords_StudentId",
                table: "PresenceRecords",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_UserId",
                table: "Student",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupRelation_GroupId",
                table: "StudentGroupRelation",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupRelation_StudentId",
                table: "StudentGroupRelation",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PresenceRecords_Student_StudentId",
                table: "PresenceRecords",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PresenceRecords_Student_StudentId",
                table: "PresenceRecords");

            migrationBuilder.DropTable(
                name: "StudentGroupRelation");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropIndex(
                name: "IX_PresenceRecords_StudentId",
                table: "PresenceRecords");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "PresenceRecords");

            migrationBuilder.AddColumn<string>(
                name: "Index",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
