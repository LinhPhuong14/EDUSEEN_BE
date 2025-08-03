using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories.impl;
using Sep490_Eduseen_BE.Repositories;
using Sep490_Eduseen_BE.Dtos;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Dtos.Teacher;
using Sep490_Eduseen_BE.Dtos.Submission;

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
                        .ThenInclude(l => l.Assignments)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return null;

            return ToDto(course);
        }

        public async Task<IEnumerable<CourseDTO>> GetCoursesAsync(int teacherId)
        {
            var courses = await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                        .ThenInclude(l => l.Assignments)
                .ToListAsync();

            return courses.Select(ToDto);
        }

        public async Task<CourseDTO> CreateCourseAsync(CreateCourseDTO dto, int teacherId)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Level = dto.Level,
                Cover = dto.Cover, // Lưu cover
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
                        .ThenInclude(l => l.Assignments)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return false;

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.CategoryId = dto.CategoryId;
            course.Level = dto.Level;
            course.Cover = dto.Cover; // Lưu cover
            course.UpdatedAt = DateTime.UtcNow;

            var sectionDict = course.Sections.ToDictionary(s => s.SectionId);

            var dtoSectionIds = dto.Sections.Where(s => s.SectionId.HasValue).Select(s => s.SectionId.Value).ToHashSet();

            var sectionsToRemove = course.Sections.Where(s => !dtoSectionIds.Contains(s.SectionId)).ToList();
            foreach (var section in sectionsToRemove)
            {
                foreach (var lecture in section.Lectures)
                {
                    _context.Assignments.RemoveRange(lecture.Assignments);
                }
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
                    foreach (var lecture in lecturesToRemove)
                    {
                        _context.Assignments.RemoveRange(lecture.Assignments);
                    }
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

        public async Task<IEnumerable<AssignmentOverviewDto>> GetAssignmentsAsync(int courseId, int teacherId)
        {
            var isOwner = await _context.Courses.AnyAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);
            if (!isOwner)
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập khoá học này.");

            var totalAssigned = await _context.Enrollments.CountAsync(e => e.CourseId == courseId);

            var assignments = await _context.Assignments
                .Include(a => a.Lecture)
                    .ThenInclude(l => l.Section)
                .Where(a => a.Lecture.Section.CourseId == courseId)
                .ToListAsync();

            var assignmentIds = assignments.Select(a => a.AssignmentId).ToList();

            var submissionStats = await _context.Submissions
                .Where(s => assignmentIds.Contains(s.AssignmentId))
                .GroupBy(s => s.AssignmentId)
                .Select(g => new
                {
                    AssignmentId = g.Key,
                    TotalSubmitted = g.Select(s => s.StudentId).Distinct().Count(),
                    AverageGrade = g.Where(s => s.Grade != null).Average(s => (double?)s.Grade) ?? 0.0
                })
                .ToListAsync();

            var statDict = submissionStats.ToDictionary(s => s.AssignmentId);

            return assignments.Select(a =>
            {
                statDict.TryGetValue(a.AssignmentId, out var stats);
                return new AssignmentOverviewDto
                {
                    AssignmentId = a.AssignmentId,
                    Title = a.Title,
                    SectionTitle = a.Lecture.Section.Title,
                    LectureTitle = a.Lecture.Title,
                    TotalAssigned = totalAssigned,
                    TotalSubmitted = stats?.TotalSubmitted ?? 0,
                    AverageGrade = stats?.AverageGrade
                };
            }).ToList();
        }

        public async Task<AssignmentSubmissionsDto> GetAssignmentSubmissionsAsync(int assignmentId, int teacherId)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Lecture)
                    .ThenInclude(l => l.Section)
                        .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
                throw new Exception("Assignment not found");

            if (assignment.Lecture.Section.Course.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập các bài nộp của bài tập này.");

            var submissions = await _context.Submissions
                .Include(s => s.Student)
                .Where(s => s.AssignmentId == assignmentId)
                .OrderByDescending(s => s.SubmittedAt)
                .Select(s => new SubmissionListItemDto
                {
                    SubmissionId = s.SubmissionId,
                    StudentId = s.StudentId,
                    StudentName = (s.Student.FirstName ?? "") + " " + (s.Student.LastName ?? ""),
                    AttemptNumber = s.AttemptNumber,
                    SubmittedAt = s.SubmittedAt,
                    Grade = s.Grade,
                    Feedback = s.Feedback
                })
                .ToListAsync();

            // Get students who have not submitted
            var courseId = assignment.Lecture.Section.CourseId;
            var enrolledStudents = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .Select(e => e.Student)
                .ToListAsync();

            var submittedIds = submissions.Select(s => s.StudentId).ToHashSet();

            var notSubmitted = enrolledStudents
                .Where(u => !submittedIds.Contains(u.UserId))
                .Select(u => new StudentInfoDto
                {
                    StudentId = u.UserId,
                    Name = (u.FirstName ?? "") + " " + (u.LastName ?? ""),
                    Email = u.Email
                })
                .ToList();

            return new AssignmentSubmissionsDto
            {
                AssignmentId = assignment.AssignmentId,
                Title = assignment.Title,
                Submissions = submissions,
                NotSubmittedStudents = notSubmitted
            };
        }

        public async Task<bool> DeleteCourseAsync(int courseId, int teacherId)
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                        .ThenInclude(l => l.Assignments)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return false;

            // Xoá các bản ghi liên quan trước khi xoá Course
            var enrollments = _context.Enrollments.Where(e => e.CourseId == courseId);
            _context.Enrollments.RemoveRange(enrollments);

            var reviews = _context.Reviews.Where(r => r.CourseId == courseId);
            _context.Reviews.RemoveRange(reviews);

            // Xoá UserLectureProgress liên quan đến các bài giảng của khoá học
            var lectureIds = course.Sections.SelectMany(s => s.Lectures).Select(l => l.LectureId).ToList();
            var userLectureProgresses = _context.UserLectureProgresses.Where(p => lectureIds.Contains(p.LectureId));
            _context.UserLectureProgresses.RemoveRange(userLectureProgresses);

            var favorites = _context.Favorites.Where(f => f.CourseId == courseId);
            _context.Favorites.RemoveRange(favorites);

            var schedules = _context.Schedules.Where(s => s.CourseId == courseId);
            _context.Schedules.RemoveRange(schedules);

            var classCourses = _context.ClassCourses.Where(cc => cc.CourseId == courseId);
            _context.ClassCourses.RemoveRange(classCourses);

            // Lấy toàn bộ lectureId thuộc course
            // var lectureIds = course.Sections.SelectMany(s => s.Lectures).Select(l => l.LectureId).ToList(); // XÓA DÒNG NÀY

            // Xoá tất cả assignment của các lecture này
            var assignments = _context.Assignments.Where(a => lectureIds.Contains(a.LectureId)).ToList();

            // Xoá tất cả submission của các assignment này
            var assignmentIds = assignments.Select(a => a.AssignmentId).ToList();
            var submissions = _context.Submissions.Where(s => assignmentIds.Contains(s.AssignmentId));
            _context.Submissions.RemoveRange(submissions);

            // Xoá assignment
            _context.Assignments.RemoveRange(assignments);

            // Xoá lectures, sections, rồi course
            foreach (var section in course.Sections)
            {
                foreach (var lecture in section.Lectures)
                {
                    _context.Assignments.RemoveRange(lecture.Assignments);
                }
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

            // Lấy danh sách assignment thuộc course này
            var assignments = await _context.Assignments
                .Where(a => a.Lecture.Section.CourseId == courseId)
                .Include(a => a.Submissions)
                .ToListAsync();

            var assignmentAnalysis = new List<AssignmentAnalysisDTO>();
            foreach (var assignment in assignments)
            {
                var totalAssigned = totalEnrollments;
                var totalSubmitted = assignment.Submissions
                    .Select(s => s.StudentId)
                    .Distinct()
                    .Count();
                var completionRateA = totalAssigned > 0 ? (double)totalSubmitted / totalAssigned : 0.0;
                var lateSubmissionCount = assignment.Submissions.Count(s => s.SubmittedAt != null && assignment.DueDate != null && s.SubmittedAt > assignment.DueDate);
                var lateSubmissionRate = totalSubmitted > 0 ? (double)lateSubmissionCount / totalSubmitted : 0.0;
                var graded = assignment.Submissions.Where(s => s.Grade.HasValue).ToList();
                var averageGrade = graded.Count > 0 ? (double)graded.Average(s => s.Grade.Value) : 0.0;
                var gradedCount = graded.Count;
                // Phân bổ điểm: 0-5, 5-7, 7-8, 8-9, 9-10
                var gradeDistribution = new Dictionary<string, int>
                {
                    { "0-5", graded.Count(s => s.Grade >= 0 && s.Grade < 5) },
                    { "5-7", graded.Count(s => s.Grade >= 5 && s.Grade < 7) },
                    { "7-8", graded.Count(s => s.Grade >= 7 && s.Grade < 8) },
                    { "8-9", graded.Count(s => s.Grade >= 8 && s.Grade < 9) },
                    { "9-10", graded.Count(s => s.Grade >= 9 && s.Grade <= 10) },
                };
                assignmentAnalysis.Add(new AssignmentAnalysisDTO
                {
                    AssignmentId = assignment.AssignmentId,
                    Title = assignment.Title,
                    TotalAssigned = totalAssigned,
                    TotalSubmitted = totalSubmitted,
                    CompletionRate = completionRateA,
                    LateSubmissionCount = lateSubmissionCount,
                    LateSubmissionRate = lateSubmissionRate,
                    AverageGrade = averageGrade,
                    GradeDistribution = gradeDistribution,
                    GradedCount = gradedCount
                });
            }

            return new CourseAnalysisDTO
            {
                CourseId = courseId,
                TotalEnrollments = totalEnrollments,
                CompletionRate = completionRate,
                AverageRating = averageRating,
                AvgCompletedLectures = avgCompletedLectures,
                Assignments = assignmentAnalysis
            };
        }

        public async Task<AssignmentAnalysisDTO> GetHomeworkAnalysisAsync(int assignmentId, int teacherId)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Lecture)
                    .ThenInclude(l => l.Section)
                        .ThenInclude(s => s.Course)
                            .ThenInclude(c => c.Enrollments)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null) throw new Exception("Assignment not found");
            if (assignment.Lecture.Section.Course.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bạn không có quyền xem phân tích bài tập này.");

            var totalAssigned = assignment.Lecture.Section.Course.Enrollments.Count;

            var submissions = await _context.Submissions
                .Where(s => s.AssignmentId == assignmentId)
                .GroupBy(s => s.StudentId)
                .Select(g => g.OrderByDescending(s => s.AttemptNumber).FirstOrDefault())
                .ToListAsync();

            var totalSubmitted = submissions.Count;
            var lateSubmissionCount = submissions.Count(s => s.SubmittedAt != null && assignment.DueDate != null && s.SubmittedAt > assignment.DueDate);
            var graded = submissions.Where(s => s.Grade != null).ToList();
            var gradedCount = graded.Count;
            double completionRate = totalAssigned > 0 ? (double)totalSubmitted / totalAssigned : 0;
            double lateSubmissionRate = totalAssigned > 0 ? (double)lateSubmissionCount / totalAssigned : 0;
            double averageGrade = gradedCount > 0 ? (double)graded.Average(s => (double)s.Grade!) : 0.0;
            // Phân bổ điểm: 0-5, 5-7, 7-8, 8-9, 9-10
            var gradeDistribution = new Dictionary<string, int>
            {
                { "0-5", graded.Count(s => s.Grade >= 0 && s.Grade < 5) },
                { "5-7", graded.Count(s => s.Grade >= 5 && s.Grade < 7) },
                { "7-8", graded.Count(s => s.Grade >= 7 && s.Grade < 8) },
                { "8-9", graded.Count(s => s.Grade >= 8 && s.Grade < 9) },
                { "9-10", graded.Count(s => s.Grade >= 9 && s.Grade <= 10) },
            };

            return new AssignmentAnalysisDTO
            {
                AssignmentId = assignment.AssignmentId,
                Title = assignment.Title,
                TotalAssigned = totalAssigned,
                TotalSubmitted = totalSubmitted,
                CompletionRate = completionRate,
                LateSubmissionCount = lateSubmissionCount,
                LateSubmissionRate = lateSubmissionRate,
                AverageGrade = averageGrade,
                GradeDistribution = gradeDistribution,
                GradedCount = gradedCount
            };
        }

        private static CourseDTO ToDto(Course course) => new()
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            Level = course.Level,
            Cover = course.Cover, // Trả về cover
            TeacherId = course.TeacherId,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt,
            Sections = course.Sections.OrderBy(s => s.Order).Select(s => new SectionDTO
            {
                SectionId = s.SectionId,
                CourseId = s.CourseId,
                Title = s.Title,
                Order = s.Order,
                Lectures = s.Lectures.OrderBy(l => l.Order).Select(l => new LectureDTO
                {
                    LectureId = l.LectureId,
                    SectionId = l.SectionId,
                    Title = l.Title,
                    ContentType = l.ContentType,
                    ContentUrl = l.ContentUrl,
                    Duration = l.Duration,
                    Order = l.Order,
                            AssignmentId = l.Assignments.FirstOrDefault()?.AssignmentId
                }).ToList()
            }).ToList()
        };

        public async Task<IEnumerable<StudentGradeDTO>> GetStudentGradesAsync(int courseId, int teacherId)
        {
            // Kiểm tra quyền truy cập
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);
            
            if (course == null)
                throw new UnauthorizedAccessException("Bạn không có quyền xem thông tin khóa học này.");

            // Lấy tất cả học viên đăng ký khóa học
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();

            // Lấy tất cả bài tập của khóa học
            var assignments = await _context.Assignments
                .Include(a => a.Lecture)
                    .ThenInclude(l => l.Section)
                .Where(a => a.Lecture.Section.CourseId == courseId)
                .ToListAsync();

            var result = new List<StudentGradeDTO>();

            foreach (var enrollment in enrollments)
            {
                var studentGrades = new StudentGradeDTO
                {
                    StudentId = enrollment.StudentId,
                    StudentName = $"{enrollment.Student.FirstName} {enrollment.Student.LastName}".Trim(),
                    StudentEmail = enrollment.Student.Email,
                    AssignmentGrades = new List<AssignmentGradeDTO>(),
                    TotalAssignments = assignments.Count,
                    CompletedAssignments = 0
                };

                double totalGrade = 0;
                int gradedCount = 0;

                foreach (var assignment in assignments)
                {
                    // Lấy submission mới nhất của học viên cho bài tập này
                    var submission = await _context.Submissions
                        .Where(s => s.AssignmentId == assignment.AssignmentId && s.StudentId == enrollment.StudentId)
                        .OrderByDescending(s => s.AttemptNumber)
                        .FirstOrDefaultAsync();

                    var assignmentGrade = new AssignmentGradeDTO
                    {
                        AssignmentId = assignment.AssignmentId,
                        AssignmentTitle = assignment.Title,
                        Grade = submission?.Grade.HasValue == true ? (double?)submission.Grade.Value : null,
                        SubmittedAt = submission?.SubmittedAt,
                        Status = submission == null ? "Not Submitted" : 
                                submission.Grade.HasValue ? "Graded" : "Submitted"
                    };

                    studentGrades.AssignmentGrades.Add(assignmentGrade);

                    if (submission != null)
                    {
                        studentGrades.CompletedAssignments++;
                        if (submission.Grade.HasValue)
                        {
                            totalGrade += (double)submission.Grade.Value;
                            gradedCount++;
                        }
                    }
                }

                // Tính điểm trung bình
                studentGrades.AverageGrade = gradedCount > 0 ? totalGrade / gradedCount : 0;
                result.Add(studentGrades);
            }

            return result.OrderBy(s => s.StudentName);
        }

    }

}
