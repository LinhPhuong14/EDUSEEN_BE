using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Sep490_Eduseen_BE.Models;

public partial class Sep490EduseenContext : DbContext
{
    public Sep490EduseenContext()
    {
    }

    public Sep490EduseenContext(DbContextOptions<Sep490EduseenContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassCourse> ClassCourses { get; set; }

    public virtual DbSet<ClassStudent> ClassStudents { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Lecture> Lectures { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ReviewResponse> ReviewResponses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<SubmissionFile> SubmissionFiles { get; set; }

    public virtual DbSet<Transcript> Transcripts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLectureProgress> UserLectureProgresses { get; set; }

    public virtual DbSet<VideoCall> VideoCalls { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            var config = new ConfigurationBuilder();
            optionsBuilder.UseSqlServer(config.AddJsonFile("appsettings.json").Build().GetConnectionString(" DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Assignme__DA891814A1C93F74");

            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.LectureId).HasColumnName("lecture_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Assignmen__creat__6FE99F9F");

            entity.HasOne(d => d.Lecture).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.LectureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assignments_Lectures");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__D54EE9B418C08AE4");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__5189E2559D5AC86F").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("category_name");
            entity.Property(e => e.Cover)
                .HasMaxLength(255)
                .HasColumnName("cover");
            entity.Property(e => e.HoverCover)
                .HasMaxLength(255)
                .HasColumnName("hoverCover");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__ChatMess__0BBF6EE6ADBF4193");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.MessageText).HasColumnName("message_text");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("sent_at");

            entity.HasOne(d => d.Course).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatMessa__cours__71D1E811");

            entity.HasOne(d => d.Sender).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatMessa__sende__72C60C4A");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__FDF479863070C48A");

            entity.HasIndex(e => e.ClassCode, "UQ__Classes__0AF9B2E49F216E04").IsUnique();

            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ClassCode)
                .HasMaxLength(20)
                .HasColumnName("class_code");
            entity.Property(e => e.ClassName)
                .HasMaxLength(100)
                .HasColumnName("class_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Classes)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classes__teacher__75A278F5");
        });

        modelBuilder.Entity<ClassCourse>(entity =>
        {
            entity.HasKey(e => e.ClassCourseId).HasName("PK__ClassCou__869DCFE1F54FE670");

            entity.HasIndex(e => new { e.ClassId, e.CourseId }, "UC_ClassCourses").IsUnique();

            entity.Property(e => e.ClassCourseId).HasColumnName("class_course_id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassCourses)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassCour__class__73BA3083");

            entity.HasOne(d => d.Course).WithMany(p => p.ClassCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassCour__cours__74AE54BC");
        });

        modelBuilder.Entity<ClassStudent>(entity =>
        {
            entity.HasKey(e => e.ClassStudentId).HasName("PK__ClassStu__86B74A0B1F7DEE88");

            entity.HasIndex(e => new { e.ClassId, e.StudentId }, "UC_ClassStudents").IsUnique();

            entity.Property(e => e.ClassStudentId).HasColumnName("class_student_id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("joined_at");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassStudents)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassStud__class__76969D2E");

            entity.HasOne(d => d.Student).WithMany(p => p.ClassStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassStud__stude__778AC167");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__8F1EF7AE69D74448");

            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Level)
                .HasMaxLength(50)
                .HasColumnName("level");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Category).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Courses__categor__787EE5A0");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Courses__teacher__797309D9");
        });

        modelBuilder.Entity<EmailConfirmationToken>(entity =>
        {
            entity.HasKey(e => e.EmailConfirmId);

            entity.ToTable("EmailConfirmationToken");

            entity.Property(e => e.EmailConfirmId).HasColumnName("email_confirm_id");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.EmailConfirmationTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailConfirmationToken_Users");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__6D24AA7A4D556B94");

            entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.EnrolledAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("enrolled_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__cours__7B5B524B");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__stude__7C4F7684");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__46ACF4CBA08BD822");

            entity.Property(e => e.FavoriteId).HasColumnName("favorite_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__cours__7D439ABD");

            entity.HasOne(d => d.Student).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__stude__7E37BEF6");
        });

        modelBuilder.Entity<Lecture>(entity =>
        {
            entity.HasKey(e => e.LectureId).HasName("PK__Lectures__797827F592D578B3");

            entity.Property(e => e.LectureId).HasColumnName("lecture_id");
            entity.Property(e => e.ContentType)
                .HasMaxLength(50)
                .HasColumnName("content_type");
            entity.Property(e => e.ContentUrl)
                .HasMaxLength(255)
                .HasColumnName("content_url");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Order).HasColumnName("order");
            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Section).WithMany(p => p.Lectures)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lectures__sectio__7F2BE32F");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842F6CF3E50D");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__user___00200768");
        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.HasKey(e => e.OtpId).HasName("PK__Otps__AEE354356962EF9E");

            entity.Property(e => e.OtpId).HasColumnName("otp_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasDefaultValue("")
                .HasColumnName("email");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false)
                .HasColumnName("is_used");
            entity.Property(e => e.OtpCode)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("otp_code");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Otps)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Otps__user_id__43D61337");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__Password__CB3C9E1762132C3D");

            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PasswordR__user___4222D4EF");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__60883D90D19D2FE9");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__course___04E4BC85");

            entity.HasOne(d => d.Student).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__student__05D8E0BE");
        });

        modelBuilder.Entity<ReviewResponse>(entity =>
        {
            entity.HasKey(e => e.ResponseId).HasName("PK__ReviewRe__EBECD896F73A9B57");

            entity.Property(e => e.ResponseId).HasColumnName("response_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.ResponseText).HasColumnName("response_text");
            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewResponses)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReviewRes__revie__02FC7413");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ReviewResponses)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReviewRes__teach__03F0984C");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CC593F1B95");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__783254B13C5F65D5").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__C46A8A6FA3E1EF97");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.ScheduledTime).HasColumnName("scheduled_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_schedules_courses");

            entity.HasOne(d => d.Student).WithMany(p => p.ScheduleStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedules__stude__06CD04F7");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ScheduleTeachers)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedules__teach__07C12930");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("PK__Sections__F842676A15305C05");

            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.Order).HasColumnName("order");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Course).WithMany(p => p.Sections)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sections__course__09A971A2");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submissi__9B53559566BFE60B");

            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.AttemptNumber).HasColumnName("attempt_number");
            entity.Property(e => e.Feedback).HasColumnName("feedback");
            entity.Property(e => e.Grade)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("grade");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.SubmissionContent).HasColumnName("submission_content");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__assig__0B91BA14");

            entity.HasOne(d => d.Student).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__stude__0C85DE4D");
        });

        modelBuilder.Entity<Submission>()
                .HasMany(s => s.SubmissionFiles)
                .WithOne(f => f.Submission)
                .HasForeignKey(f => f.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubmissionFile>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__Submissi__07D884C6871161F7");

            entity.Property(e => e.FileId).HasColumnName("file_id");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(255)
                .HasColumnName("file_url");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");

            entity.HasOne(d => d.Submission).WithMany(p => p.SubmissionFiles)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__submi__0A9D95DB");
        });

        modelBuilder.Entity<Transcript>(entity =>
        {
            entity.HasKey(e => e.TranscriptId).HasName("PK__Transcri__3D043C38A40F5055");

            entity.Property(e => e.TranscriptId).HasColumnName("transcript_id");
            entity.Property(e => e.CallId).HasColumnName("call_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.RecordingUrl)
                .HasMaxLength(255)
                .HasColumnName("recording_url");

            entity.HasOne(d => d.Call).WithMany(p => p.Transcripts)
                .HasForeignKey(d => d.CallId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transcrip__call___0D7A0286");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FE04AFDF8");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164F20E8A03").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC572A38BECF7").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.EmailConfirmed)
                .HasDefaultValue(false)
                .HasColumnName("email_confirmed");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .IsFixedLength()
                .HasColumnName("refresh_token");
            entity.Property(e => e.RefreshTokenExpiresAt).HasColumnName("refresh_token_expires_at");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__role_id__10566F31");
        });

        modelBuilder.Entity<UserLectureProgress>(entity =>
        {
            entity.HasKey(e => e.ProgressId).HasName("PK__UserLect__49B3D8C19C487BC1");

            entity.ToTable("UserLectureProgress");

            entity.Property(e => e.ProgressId).HasColumnName("progress_id");
            entity.Property(e => e.IsCompleted)
                .HasDefaultValue(false)
                .HasColumnName("is_completed");
            entity.Property(e => e.LastAccessed).HasColumnName("last_accessed");
            entity.Property(e => e.LectureId).HasColumnName("lecture_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Lecture).WithMany(p => p.UserLectureProgresses)
                .HasForeignKey(d => d.LectureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserLectu__lectu__0E6E26BF");

            entity.HasOne(d => d.User).WithMany(p => p.UserLectureProgresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserLectu__user___0F624AF8");
        });

        modelBuilder.Entity<VideoCall>(entity =>
        {
            entity.HasKey(e => e.CallId).HasName("PK__VideoCal__427DCE684BA665E7");

            entity.Property(e => e.CallId).HasColumnName("call_id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Schedule).WithMany(p => p.VideoCalls)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VideoCall__sched__114A936A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
