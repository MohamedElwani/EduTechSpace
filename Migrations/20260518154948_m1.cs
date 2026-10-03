using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTechSpace.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxDrgree",
                table: "exams",
                newName: "MaxDegree");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxDegree",
                table: "exams",
                newName: "MaxDrgree");
        }
    }
}
