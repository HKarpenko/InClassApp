using Microsoft.EntityFrameworkCore.Migrations;

namespace InClassApp.Data.Migrations
{
    public partial class ManyLecturersForGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Lecturer_LecturerId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_LecturerId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "LecturerId",
                table: "Groups");

            migrationBuilder.CreateTable(
                name: "LecturerGroupRelation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(nullable: false),
                    LecturerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LecturerGroupRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LecturerGroupRelation_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LecturerGroupRelation_Lecturer_LecturerId",
                        column: x => x.LecturerId,
                        principalTable: "Lecturer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LecturerGroupRelation_GroupId",
                table: "LecturerGroupRelation",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LecturerGroupRelation_LecturerId",
                table: "LecturerGroupRelation",
                column: "LecturerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LecturerGroupRelation");

            migrationBuilder.AddColumn<int>(
                name: "LecturerId",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_LecturerId",
                table: "Groups",
                column: "LecturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Lecturer_LecturerId",
                table: "Groups",
                column: "LecturerId",
                principalTable: "Lecturer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
