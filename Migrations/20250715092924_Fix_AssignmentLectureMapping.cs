using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sep490_Eduseen_BE.Migrations
{
    /// <inheritdoc />
    public partial class Fix_AssignmentLectureMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Assignmen__cours__628FA481",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK__Assignmen__creat__6383C8BA",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK__Submissio__submi__797309D9",
                table: "SubmissionFiles");

            migrationBuilder.RenameColumn(
                name: "course_id",
                table: "Assignments",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_course_id",
                table: "Assignments",
                newName: "IX_Assignments_CourseId");

            migrationBuilder.AddColumn<int>(
                name: "course_id",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Lectures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByNavigationUserId",
                table: "Lectures",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Assignments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Assignments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LectureId1",
                table: "Assignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "lecture_id",
                table: "Assignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_course_id",
                table: "Schedules",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_CreatedByNavigationUserId",
                table: "Lectures",
                column: "CreatedByNavigationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_lecture_id",
                table: "Assignments",
                column: "lecture_id");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_LectureId1",
                table: "Assignments",
                column: "LectureId1",
                unique: true,
                filter: "[LectureId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Courses_CourseId",
                table: "Assignments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "course_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Lectures_LectureId1",
                table: "Assignments",
                column: "LectureId1",
                principalTable: "Lectures",
                principalColumn: "lecture_id");

            migrationBuilder.AddForeignKey(
                name: "FK__Assignment__created_by",
                table: "Assignments",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__Assignment__lecture_id",
                table: "Assignments",
                column: "lecture_id",
                principalTable: "Lectures",
                principalColumn: "lecture_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Users_CreatedByNavigationUserId",
                table: "Lectures",
                column: "CreatedByNavigationUserId",
                principalTable: "Users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Courses_course_id",
                table: "Schedules",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "course_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Submissio__submi__797309D9",
                table: "SubmissionFiles",
                column: "submission_id",
                principalTable: "Submissions",
                principalColumn: "submission_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Courses_CourseId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Lectures_LectureId1",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK__Assignment__created_by",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK__Assignment__lecture_id",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_Users_CreatedByNavigationUserId",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Courses_course_id",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK__Submissio__submi__797309D9",
                table: "SubmissionFiles");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_course_id",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Lectures_CreatedByNavigationUserId",
                table: "Lectures");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_lecture_id",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_LectureId1",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "course_id",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "CreatedByNavigationUserId",
                table: "Lectures");

            migrationBuilder.DropColumn(
                name: "LectureId1",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "lecture_id",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Assignments",
                newName: "course_id");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_CourseId",
                table: "Assignments",
                newName: "IX_Assignments_course_id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Assignments",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<int>(
                name: "course_id",
                table: "Assignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__Assignmen__cours__628FA481",
                table: "Assignments",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "course_id");

            migrationBuilder.AddForeignKey(
                name: "FK__Assignmen__creat__6383C8BA",
                table: "Assignments",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__Submissio__submi__797309D9",
                table: "SubmissionFiles",
                column: "submission_id",
                principalTable: "Submissions",
                principalColumn: "submission_id");
        }
    }
}
