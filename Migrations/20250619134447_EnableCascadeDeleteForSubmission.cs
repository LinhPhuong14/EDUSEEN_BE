using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sep490_Eduseen_BE.Migrations
{
    /// <inheritdoc />
    public partial class EnableCascadeDeleteForSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__D54EE9B4C08CD780", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__760965CC83467F9F", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    avatar_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    refresh_token = table.Column<string>(type: "nchar(255)", fixedLength: true, maxLength: 255, nullable: true),
                    refresh_token_expires_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__B9BE370F92FB6987", x => x.user_id);
                    table.ForeignKey(
                        name: "FK__Users__role_id__3F466844",
                        column: x => x.role_id,
                        principalTable: "Roles",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    class_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    class_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    class_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    teacher_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Classes__FDF47986B4015B69", x => x.class_id);
                    table.ForeignKey(
                        name: "FK__Classes__teacher__68487DD7",
                        column: x => x.teacher_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    teacher_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Courses__8F1EF7AE66FFCB25", x => x.course_id);
                    table.ForeignKey(
                        name: "FK__Courses__categor__49C3F6B7",
                        column: x => x.category_id,
                        principalTable: "Categories",
                        principalColumn: "category_id");
                    table.ForeignKey(
                        name: "FK__Courses__teacher__4AB81AF0",
                        column: x => x.teacher_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "EmailConfirmationToken",
                columns: table => new
                {
                    email_confirm_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailConfirmationToken", x => x.email_confirm_id);
                    table.ForeignKey(
                        name: "FK_EmailConfirmationToken_Users",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    is_read = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__E059842F56D909F9", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK__Notificat__user___07C12930",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Otps",
                columns: table => new
                {
                    otp_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: ""),
                    otp_code = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: false, defaultValue: ""),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    is_used = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Otps__AEE354356962EF9E", x => x.otp_id);
                    table.ForeignKey(
                        name: "FK__Otps__user_id__43D61337",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    token_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Password__CB3C9E1762132C3D", x => x.token_id);
                    table.ForeignKey(
                        name: "FK__PasswordR__user___4222D4EF",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    schedule_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    teacher_id = table.Column<int>(type: "int", nullable: false),
                    scheduled_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schedule__C46A8A6FD5BDFD86", x => x.schedule_id);
                    table.ForeignKey(
                        name: "FK__Schedules__stude__0A9D95DB",
                        column: x => x.student_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__Schedules__teach__0B91BA14",
                        column: x => x.teacher_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "ClassStudents",
                columns: table => new
                {
                    class_student_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    class_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    joined_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ClassStu__86B74A0BD1B1B359", x => x.class_student_id);
                    table.ForeignKey(
                        name: "FK__ClassStud__class__71D1E811",
                        column: x => x.class_id,
                        principalTable: "Classes",
                        principalColumn: "class_id");
                    table.ForeignKey(
                        name: "FK__ClassStud__stude__72C60C4A",
                        column: x => x.student_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    assignment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    due_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Assignme__DA891814505DEDAD", x => x.assignment_id);
                    table.ForeignKey(
                        name: "FK__Assignmen__cours__628FA481",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK__Assignmen__creat__6383C8BA",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    sender_id = table.Column<int>(type: "int", nullable: false),
                    message_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChatMess__0BBF6EE6E3BD7CD9", x => x.message_id);
                    table.ForeignKey(
                        name: "FK__ChatMessa__cours__160F4887",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK__ChatMessa__sende__17036CC0",
                        column: x => x.sender_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "ClassCourses",
                columns: table => new
                {
                    class_course_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    class_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ClassCou__869DCFE1C6D40342", x => x.class_course_id);
                    table.ForeignKey(
                        name: "FK__ClassCour__class__6C190EBB",
                        column: x => x.class_id,
                        principalTable: "Classes",
                        principalColumn: "class_id");
                    table.ForeignKey(
                        name: "FK__ClassCour__cours__6D0D32F4",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id");
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    enrollment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    enrolled_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Enrollme__6D24AA7ABE2E1AC6", x => x.enrollment_id);
                    table.ForeignKey(
                        name: "FK__Enrollmen__cours__5535A963",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK__Enrollmen__stude__5441852A",
                        column: x => x.student_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    favorite_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Favorite__46ACF4CBBA912BBF", x => x.favorite_id);
                    table.ForeignKey(
                        name: "FK__Favorites__cours__5EBF139D",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK__Favorites__stude__5DCAEF64",
                        column: x => x.student_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reviews__60883D905BD52DFC", x => x.review_id);
                    table.ForeignKey(
                        name: "FK__Reviews__course___7D439ABD",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK__Reviews__student__7E37BEF6",
                        column: x => x.student_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    section_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sections__F842676A60047FF6", x => x.section_id);
                    table.ForeignKey(
                        name: "FK__Sections__course__4D94879B",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id");
                });

            migrationBuilder.CreateTable(
                name: "VideoCalls",
                columns: table => new
                {
                    call_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    schedule_id = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VideoCal__427DCE6823EB81AC", x => x.call_id);
                    table.ForeignKey(
                        name: "FK__VideoCall__sched__0E6E26BF",
                        column: x => x.schedule_id,
                        principalTable: "Schedules",
                        principalColumn: "schedule_id");
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    submission_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    assignment_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    attempt_number = table.Column<int>(type: "int", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    submission_content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    grade = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    feedback = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Submissi__9B5355954A9C148E", x => x.submission_id);
                    table.ForeignKey(
                        name: "FK__Submissio__assig__75A278F5",
                        column: x => x.assignment_id,
                        principalTable: "Assignments",
                        principalColumn: "assignment_id");
                    table.ForeignKey(
                        name: "FK__Submissio__stude__76969D2E",
                        column: x => x.student_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "ReviewResponses",
                columns: table => new
                {
                    response_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    review_id = table.Column<int>(type: "int", nullable: false),
                    teacher_id = table.Column<int>(type: "int", nullable: false),
                    response_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReviewRe__EBECD89688310922", x => x.response_id);
                    table.ForeignKey(
                        name: "FK__ReviewRes__revie__02084FDA",
                        column: x => x.review_id,
                        principalTable: "Reviews",
                        principalColumn: "review_id");
                    table.ForeignKey(
                        name: "FK__ReviewRes__teach__02FC7413",
                        column: x => x.teacher_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Lectures",
                columns: table => new
                {
                    lecture_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    section_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    content_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    duration = table.Column<int>(type: "int", nullable: true),
                    order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Lectures__797827F5DABC7A39", x => x.lecture_id);
                    table.ForeignKey(
                        name: "FK__Lectures__sectio__5070F446",
                        column: x => x.section_id,
                        principalTable: "Sections",
                        principalColumn: "section_id");
                });

            migrationBuilder.CreateTable(
                name: "Transcripts",
                columns: table => new
                {
                    transcript_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    call_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    recording_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Transcri__3D043C3827EBCA35", x => x.transcript_id);
                    table.ForeignKey(
                        name: "FK__Transcrip__call___123EB7A3",
                        column: x => x.call_id,
                        principalTable: "VideoCalls",
                        principalColumn: "call_id");
                });

            migrationBuilder.CreateTable(
                name: "SubmissionFiles",
                columns: table => new
                {
                    file_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    submission_id = table.Column<int>(type: "int", nullable: false),
                    file_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    file_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Submissi__07D884C6EDC7756F", x => x.file_id);
                    table.ForeignKey(
                        name: "FK__Submissio__submi__797309D9",
                        column: x => x.submission_id,
                        principalTable: "Submissions",
                        principalColumn: "submission_id");
                });

            migrationBuilder.CreateTable(
                name: "UserLectureProgress",
                columns: table => new
                {
                    progress_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    lecture_id = table.Column<int>(type: "int", nullable: false),
                    is_completed = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    last_accessed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserLect__49B3D8C1021561E7", x => x.progress_id);
                    table.ForeignKey(
                        name: "FK__UserLectu__lectu__59FA5E80",
                        column: x => x.lecture_id,
                        principalTable: "Lectures",
                        principalColumn: "lecture_id");
                    table.ForeignKey(
                        name: "FK__UserLectu__user___59063A47",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_course_id",
                table: "Assignments",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_created_by",
                table: "Assignments",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "UQ__Categori__5189E2557A4064D3",
                table: "Categories",
                column: "category_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_course_id",
                table: "ChatMessages",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_sender_id",
                table: "ChatMessages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_ClassCourses_course_id",
                table: "ClassCourses",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "UC_ClassCourses",
                table: "ClassCourses",
                columns: new[] { "class_id", "course_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_teacher_id",
                table: "Classes",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Classes__0AF9B2E4AE4DCCFE",
                table: "Classes",
                column: "class_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassStudents_student_id",
                table: "ClassStudents",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "UC_ClassStudents",
                table: "ClassStudents",
                columns: new[] { "class_id", "student_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_category_id",
                table: "Courses",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_teacher_id",
                table: "Courses",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_EmailConfirmationToken_user_id",
                table: "EmailConfirmationToken",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_course_id",
                table: "Enrollments",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_student_id",
                table: "Enrollments",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_course_id",
                table: "Favorites",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_student_id",
                table: "Favorites",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_section_id",
                table: "Lectures",
                column: "section_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_user_id",
                table: "Notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Otps_user_id",
                table: "Otps",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_user_id",
                table: "PasswordResetTokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResponses_review_id",
                table: "ReviewResponses",
                column: "review_id");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResponses_teacher_id",
                table: "ReviewResponses",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_course_id",
                table: "Reviews",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_student_id",
                table: "Reviews",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__783254B1196FC9B2",
                table: "Roles",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_student_id",
                table: "Schedules",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_teacher_id",
                table: "Schedules",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_course_id",
                table: "Sections",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionFiles_submission_id",
                table: "SubmissionFiles",
                column: "submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_assignment_id",
                table: "Submissions",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_student_id",
                table: "Submissions",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_Transcripts_call_id",
                table: "Transcripts",
                column: "call_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserLectureProgress_lecture_id",
                table: "UserLectureProgress",
                column: "lecture_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserLectureProgress_user_id",
                table: "UserLectureProgress",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_role_id",
                table: "Users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__AB6E6164AB032028",
                table: "Users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Users__F3DBC572DCAFB38F",
                table: "Users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoCalls_schedule_id",
                table: "VideoCalls",
                column: "schedule_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ClassCourses");

            migrationBuilder.DropTable(
                name: "ClassStudents");

            migrationBuilder.DropTable(
                name: "EmailConfirmationToken");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Otps");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "ReviewResponses");

            migrationBuilder.DropTable(
                name: "SubmissionFiles");

            migrationBuilder.DropTable(
                name: "Transcripts");

            migrationBuilder.DropTable(
                name: "UserLectureProgress");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "VideoCalls");

            migrationBuilder.DropTable(
                name: "Lectures");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
