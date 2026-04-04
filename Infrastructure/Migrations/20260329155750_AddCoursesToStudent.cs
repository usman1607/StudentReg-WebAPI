using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoursesToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "credit_units",
                table: "courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_students_courses_course_id",
                table: "students_courses",
                column: "course_id");

            migrationBuilder.AddForeignKey(
                name: "FK_students_courses_courses_course_id",
                table: "students_courses",
                column: "course_id",
                principalTable: "courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_students_courses_users_student_id",
                table: "students_courses",
                column: "student_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_students_courses_courses_course_id",
                table: "students_courses");

            migrationBuilder.DropForeignKey(
                name: "FK_students_courses_users_student_id",
                table: "students_courses");

            migrationBuilder.DropIndex(
                name: "IX_students_courses_course_id",
                table: "students_courses");

            migrationBuilder.DropColumn(
                name: "credit_units",
                table: "courses");
        }
    }
}
