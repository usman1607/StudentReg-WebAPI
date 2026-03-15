using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstMgt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:case_insensitive", "en-u-ks-primary,en-u-ks-primary,icu,False");

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", nullable: false, collation: "case_insensitive"),
                    code = table.Column<string>(type: "varchar(20)", nullable: false, collation: "case_insensitive"),
                    description = table.Column<string>(type: "varchar(255)", nullable: true, collation: "case_insensitive"),
                    CreatedBy = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true, collation: "case_insensitive"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(50)", nullable: false, collation: "case_insensitive"),
                    description = table.Column<string>(type: "varchar(255)", nullable: true, collation: "case_insensitive"),
                    created_by = table.Column<string>(type: "varchar(255)", nullable: false, collation: "case_insensitive"),
                    modified_by = table.Column<string>(type: "varchar(255)", nullable: true, collation: "case_insensitive"),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "students_courses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<string>(type: "varchar(255)", nullable: false, collation: "case_insensitive"),
                    modified_by = table.Column<string>(type: "varchar(255)", nullable: true, collation: "case_insensitive"),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students_courses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "varchar(50)", nullable: false, collation: "case_insensitive"),
                    last_name = table.Column<string>(type: "varchar(50)", nullable: false, collation: "case_insensitive"),
                    email = table.Column<string>(type: "varchar(100)", nullable: false, collation: "case_insensitive"),
                    phone_number = table.Column<string>(type: "varchar(50)", nullable: false, collation: "case_insensitive"),
                    password_hash = table.Column<string>(type: "varchar(255)", nullable: false, collation: "case_insensitive"),
                    hash_salt = table.Column<string>(type: "varchar(255)", nullable: false, collation: "case_insensitive"),
                    user_type_enum = table.Column<string>(type: "varchar(50)", nullable: false),
                    address = table.Column<string>(type: "varchar(255)", nullable: false, collation: "case_insensitive"),
                    gender = table.Column<string>(type: "varchar(50)", nullable: false),
                    user_type = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false, collation: "case_insensitive"),
                    staff_number = table.Column<string>(type: "varchar(20)", nullable: true, collation: "case_insensitive"),
                    delegation = table.Column<string>(type: "varchar(50)", nullable: true),
                    matric_number = table.Column<string>(type: "varchar(20)", nullable: true, collation: "case_insensitive"),
                    status = table.Column<string>(type: "varchar(50)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(255)", nullable: false, collation: "case_insensitive"),
                    modified_by = table.Column<string>(type: "varchar(255)", nullable: true, collation: "case_insensitive"),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<string>(type: "varchar(255)", nullable: false, collation: "case_insensitive"),
                    modified_by = table.Column<string>(type: "varchar(255)", nullable: true, collation: "case_insensitive"),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_courses_code",
                table: "courses",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_students_courses_student_id_course_id",
                table: "students_courses",
                columns: new[] { "student_id", "course_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_user_id_role_id",
                table: "user_roles",
                columns: new[] { "user_id", "role_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_matric_number",
                table: "users",
                column: "matric_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_staff_number",
                table: "users",
                column: "staff_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "students_courses");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
