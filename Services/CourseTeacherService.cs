using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories.impl;
using Sep490_Eduseen_BE.Repositories;
using Sep490_Eduseen_BE.Dtos;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Dtos.Teacher;

namespace Sep490_Eduseen_BE.Services
{
    public class CourseTeacherService : ICourseTeacherService
    {
        private readonly Sep490EduseenContext _context;

        public CourseTeacherService(Sep490EduseenContext context)
        {
            _context = context;
        }

        public async Task<CourseDTO?> GetCourseAsync(int courseId, int teacherId)
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return null;

            return ToDto(course);
        }

        public async Task<CourseDTO> CreateCourseAsync(CreateCourseDTO dto, int teacherId)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Level = dto.Level,
                TeacherId = teacherId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Sections = dto.Sections.Select(s => new Section
                {
                    Title = s.Title,
                    Order = s.Order,
                    Lectures = s.Lectures.Select(l => new Lecture
                    {
                        Title = l.Title,
                        ContentType = l.ContentType,
                        ContentUrl = l.ContentUrl,
                        Duration = l.Duration,
                        Order = l.Order
                    }).ToList()
                }).ToList()
            };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return ToDto(course);
        }

        public async Task<bool> UpdateCourseAsync(int courseId, UpdateCourseDTO dto, int teacherId)
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return false;

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.CategoryId = dto.CategoryId;
            course.Level = dto.Level;
            course.UpdatedAt = DateTime.UtcNow;

            var sectionDict = course.Sections.ToDictionary(s => s.SectionId);

            var dtoSectionIds = dto.Sections.Where(s => s.SectionId.HasValue).Select(s => s.SectionId.Value).ToHashSet();

            var sectionsToRemove = course.Sections.Where(s => !dtoSectionIds.Contains(s.SectionId)).ToList();
            foreach (var section in sectionsToRemove)
            {
                _context.Lectures.RemoveRange(section.Lectures);
                _context.Sections.Remove(section);
            }

            foreach (var sectionDto in dto.Sections)
            {
                Section section;
                if (sectionDto.SectionId.HasValue && sectionDict.TryGetValue(sectionDto.SectionId.Value, out section!))
                {
                    section.Title = sectionDto.Title;
                    section.Order = sectionDto.Order;

                    var lectureDict = section.Lectures.ToDictionary(l => l.LectureId);
                    var dtoLectureIds = sectionDto.Lectures.Where(l => l.LectureId.HasValue).Select(l => l.LectureId.Value).ToHashSet();

                    var lecturesToRemove = section.Lectures.Where(l => !dtoLectureIds.Contains(l.LectureId)).ToList();
                    _context.Lectures.RemoveRange(lecturesToRemove);

                    foreach (var lectureDto in sectionDto.Lectures)
                    {
                        if (lectureDto.LectureId.HasValue && lectureDict.TryGetValue(lectureDto.LectureId.Value, out var lecture))
                        {
                            lecture.Title = lectureDto.Title;
                            lecture.ContentType = lectureDto.ContentType;
                            lecture.ContentUrl = lectureDto.ContentUrl;
                            lecture.Duration = lectureDto.Duration;
                            lecture.Order = lectureDto.Order;
                        }
                        else
                        {
                            section.Lectures.Add(new Lecture
                            {
                                Title = lectureDto.Title,
                                ContentType = lectureDto.ContentType,
                                ContentUrl = lectureDto.ContentUrl,
                                Duration = lectureDto.Duration,
                                Order = lectureDto.Order
                            });
                        }
                    }
                }
                else
                {
                    var newSection = new Section
                    {
                        Title = sectionDto.Title,
                        Order = sectionDto.Order,
                        Lectures = sectionDto.Lectures.Select(l => new Lecture
                        {
                            Title = l.Title,
                            ContentType = l.ContentType,
                            ContentUrl = l.ContentUrl,
                            Duration = l.Duration,
                            Order = l.Order
                        }).ToList()
                    };
                    course.Sections.Add(newSection);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteCourseAsync(int courseId, int teacherId)
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return false;

            // Xóa lectures, sections, rồi course
            foreach (var section in course.Sections)
            {
                _context.Lectures.RemoveRange(section.Lectures);
            }
            _context.Sections.RemoveRange(course.Sections);
            _context.Courses.Remove(course);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CourseAnalysisDTO> GetCourseAnalysisAsync(int courseId, int teacherId)
        {
            // Đảm bảo course thuộc về teacher này
            var isOwner = await _context.Courses.AnyAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);
            if (!isOwner)
                throw new UnauthorizedAccessException("Bạn không có quyền xem phân tích khóa học này.");

            var totalEnrollments = await _context.Enrollments.CountAsync(e => e.CourseId == courseId);

            double completionRate = 0.0;
            if (totalEnrollments > 0)
            {
                var completed = await _context.Enrollments.CountAsync(e => e.CourseId == courseId && e.Status == "completed");
                completionRate = (double)completed / totalEnrollments;
            }

            var averageRating = await _context.Reviews
                .Where(r => r.CourseId == courseId)
                .AverageAsync(r => (double?)r.Rating) ?? 0.0;

            var studentIds = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToListAsync();

            double avgCompletedLectures = 0.0;
            if (studentIds.Count > 0)
            {
                var completedLectures = await _context.UserLectureProgresses 
                    .Where(p => studentIds.Contains(p.UserId) && p.IsCompleted == true)
                    .GroupBy(p => p.UserId)
                    .Select(g => g.Count())
                    .ToListAsync();

                if (completedLectures.Count > 0)
                    avgCompletedLectures = completedLectures.Average();
            }

            return new CourseAnalysisDTO
            {
                CourseId = courseId,
                TotalEnrollments = totalEnrollments,
                CompletionRate = completionRate,
                AverageRating = averageRating,
                AvgCompletedLectures = avgCompletedLectures
            };
        }


        private static CourseDTO ToDto(Course course) => new()
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            Level = course.Level,
            TeacherId = course.TeacherId,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt,
            Sections = course.Sections
                .OrderBy(s => s.Order)
                .Select(s => new SectionDTO
                {
                    SectionId = s.SectionId,
                    CourseId = s.CourseId,
                    Title = s.Title,
                    Order = s.Order,
                    Lectures = s.Lectures
                        .OrderBy(l => l.Order)
                        .Select(l => new LectureDTO
                        {
                            LectureId = l.LectureId,
                            SectionId = l.SectionId,
                            Title = l.Title,
                            ContentType = l.ContentType,
                            ContentUrl = l.ContentUrl,
                            Duration = l.Duration,
                            Order = l.Order
                        }).ToList()
                }).ToList()
        };
        public async Task<HomeworkAnalysisDto> GetHomeworkAnalysisAsync(int assignmentId, int teacherId)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                    .ThenInclude(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null) throw new Exception("Assignment not found");
            if (assignment.Course.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bạn không có quyền xem phân tích bài tập này.");

            var totalAssigned = assignment.Course.Enrollments.Count;

            var submissions = await _context.Submissions
                .Where(s => s.AssignmentId == assignmentId)
                .GroupBy(s => s.StudentId)
                .Select(g => g.OrderByDescending(s => s.AttemptNumber).FirstOrDefault())
                .ToListAsync();

            var totalSubmitted = submissions.Count;

            var lateSubmissionCount = submissions.Count(s => s.SubmittedAt != null && assignment.DueDate != null && s.SubmittedAt > assignment.DueDate);

            var gradedCount = submissions.Count(s => s.Grade != null);

            double completionRate = totalAssigned > 0 ? (double)totalSubmitted / totalAssigned : 0;
            double lateSubmissionRate = totalAssigned > 0 ? (double)lateSubmissionCount / totalAssigned : 0;
            double? averageGrade = submissions.Where(s => s.Grade != null).Any()
                ? (double?)submissions.Where(s => s.Grade != null).Average(s => (double)s.Grade!)
                : null;

            var gradeDistribution = submissions
                .Where(s => s.Grade != null)
                .GroupBy(s =>
                {
                    var grade = (double)s.Grade!;
                    if (grade < 6) return "0-5";
                    if (grade < 8) return "6-7";
                    return "8-10";
                })
                .ToDictionary(g => g.Key, g => g.Count());

            var submittedStudentIds = submissions.Select(s => s.StudentId).ToHashSet();
            var notSubmittedStudents = assignment.Course.Enrollments
                .Where(e => !submittedStudentIds.Contains(e.StudentId))
                .Select(e => new StudentInfoDto
                {
                    StudentId = e.StudentId,
                    Name = (e.Student.FirstName ?? "") + " " + (e.Student.LastName ?? ""),
                    Email = e.Student.Email
                }).ToList();

            return new HomeworkAnalysisDto
            {
                AssignmentId = assignmentId,
                TotalAssigned = totalAssigned,
                TotalSubmitted = totalSubmitted,
                CompletionRate = completionRate,
                LateSubmissionCount = lateSubmissionCount,
                LateSubmissionRate = lateSubmissionRate,
                AverageGrade = averageGrade,
                GradeDistribution = gradeDistribution,
                GradedCount = gradedCount,
                NotSubmittedStudents = notSubmittedStudents
            };
        }

    }

}
