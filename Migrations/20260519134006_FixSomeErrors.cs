using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTechSpace.Migrations
{
    /// <inheritdoc />
    public partial class FixSomeErrors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "ApplicationUserExam");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_UserCourse",
            //    table: "UserCourse");

            //migrationBuilder.DropIndex(
            //    name: "IX_UserCourse_UserId",
            //    table: "UserCourse");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_UserCourse",
            //    table: "UserCourse",
            //    columns: new[] { "UserId", "CourseId" });

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserCourse_CourseId",
            //    table: "UserCourse",
            //    column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCourse",
                table: "UserCourse");

            migrationBuilder.DropIndex(
                name: "IX_UserCourse_CourseId",
                table: "UserCourse");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCourse",
                table: "UserCourse",
                columns: new[] { "CourseId", "UserId" });

            migrationBuilder.CreateTable(
                name: "ApplicationUserExam",
                columns: table => new
                {
                    ExamsId = table.Column<int>(type: "int", nullable: false),
                    StudentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserExam", x => new { x.ExamsId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserExam_AspNetUsers_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserExam_exams_ExamsId",
                        column: x => x.ExamsId,
                        principalTable: "exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCourse_UserId",
                table: "UserCourse",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserExam_StudentsId",
                table: "ApplicationUserExam",
                column: "StudentsId");
        }
    }
}
