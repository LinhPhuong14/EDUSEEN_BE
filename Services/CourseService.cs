using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories.impl;
using Sep490_Eduseen_BE.Repositories;
using Sep490_Eduseen_BE.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Sep490_Eduseen_BE.Services
{
    public class CourseService : ICourseService
    {
        private readonly Sep490EduseenContext _context;

        public CourseService(Sep490EduseenContext context)
        {
            _context = context;
        }

        public async Task<CourseDto?> GetCourseAsync(int courseId, int teacherId)
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return null;

            return ToDto(course);
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, int teacherId)
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

        public async Task<bool> UpdateCourseAsync(int courseId, UpdateCourseDto dto, int teacherId)
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

        public async Task<CourseAnalysisDto> GetCourseAnalysisAsync(int courseId, int teacherId)
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

            return new CourseAnalysisDto
            {
                CourseId = courseId,
                TotalEnrollments = totalEnrollments,
                CompletionRate = completionRate,
                AverageRating = averageRating,
                AvgCompletedLectures = avgCompletedLectures
            };
        }


        private static CourseDto ToDto(Course course) => new()
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
                .Select(s => new SectionDto
                {
                    SectionId = s.SectionId,
                    CourseId = s.CourseId,
                    Title = s.Title,
                    Order = s.Order,
                    Lectures = s.Lectures
                        .OrderBy(l => l.Order)
                        .Select(l => new LectureDto
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
    }
}
