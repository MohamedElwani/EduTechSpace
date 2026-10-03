using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTechSpace.Migrations
{
    /// <inheritdoc />
    public partial class FixExtraCourseForigenKeyInExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exams_Courses_CoursesId",
                table: "exams");

            migrationBuilder.DropIndex(
                name: "IX_exams_CoursesId",
                table: "exams");

            migrationBuilder.DropColumn(
                name: "CoursesId",
                table: "exams");

            migrationBuilder.AddColumn<int>(
                name: "accountType",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_exams_CourseId",
                table: "exams",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_exams_Courses_CourseId",
                table: "exams",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exams_Courses_CourseId",
                table: "exams");

            migrationBuilder.DropIndex(
                name: "IX_exams_CourseId",
                table: "exams");

            migrationBuilder.DropColumn(
                name: "accountType",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "CoursesId",
                table: "exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_exams_CoursesId",
                table: "exams",
                column: "CoursesId");

            migrationBuilder.AddForeignKey(
                name: "FK_exams_Courses_CoursesId",
                table: "exams",
                column: "CoursesId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
