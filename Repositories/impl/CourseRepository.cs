using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Dtos.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Repositories.impl
{
    public class CourseRepository : ICourseRepository
    {
        private readonly Sep490EduseenContext _context;

        public CourseRepository(Sep490EduseenContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> SearchCoursesAsync(string courseName)
        {
            var lowerCaseCourseName = courseName.Trim().ToLower();
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Where(c => c.Title.ToLower().Contains(lowerCaseCourseName))
                .ToListAsync();
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Sections.OrderBy(s => s.Order))
                    .ThenInclude(s => s.Lectures.OrderBy(l => l.Order))
                        .ThenInclude(l => l.Assignments)
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.Student)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<IEnumerable<Course>> GetEnrolledCoursesByStudentIdAsync(int studentId)
        {
            return await _context.Courses
                .Where(c => c.Enrollments.Any(e => e.StudentId == studentId))
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                        .ThenInclude(l => l.Assignments)
                .ToListAsync();
        }

        public async Task<Favorite> GetFavoriteAsync(int studentId, int courseId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.StudentId == studentId && f.CourseId == courseId);
        }

        public async Task AddFavoriteAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserEnrolledAsync(int studentId, int courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task<bool> IsUserEnrolledWithStatusAsync(int studentId, int courseId, string status)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.Status == status);
        }

        public async Task<IEnumerable<Lecture>> GetLecturesByCourseIdAsync(int courseId)
        {
            return await _context.Lectures
                .Include(l => l.Assignments)
                .Where(l => l.Section.CourseId == courseId)
                .OrderBy(l => l.Order)
                .ToListAsync();
        }

        public async Task<int> GetCompletedLecturesCountAsync(int studentId, int courseId)
        {
            return await _context.UserLectureProgresses
                .CountAsync(p => p.UserId == studentId && p.IsCompleted == true && p.Lecture.Section.CourseId == courseId);
        }

        public async Task<Review> GetReviewAsync(int studentId, int courseId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.StudentId == studentId && r.CourseId == courseId);
        }

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Review>> GetTopReviewsAsync(int count = 3)
        {
            return await _context.Reviews
                .Include(r => r.Student)
                .Include(r => r.Course)
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId)
        {
            return await _context.Courses
                .Where(c => c.CategoryId == categoryId)
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Reviews)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                        .ThenInclude(l => l.Assignments)
                .Include(c => c.Favorites)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetTopCoursesAsync(int count)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Reviews)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                        .ThenInclude(l => l.Assignments)
                .Include(c => c.Favorites)
                .OrderByDescending(c => c.Reviews.Count)
                .ThenByDescending(c => c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Dictionary<int, bool>> GetLectureCompletionStatusAsync(int studentId, int courseId)
        {
            var progressData = await _context.UserLectureProgresses
                .Where(p => p.UserId == studentId && p.Lecture.Section.CourseId == courseId)
                .Select(p => new { p.LectureId, p.IsCompleted })
                .ToListAsync();

            return progressData.ToDictionary(p => p.LectureId, p => p.IsCompleted ?? false);
        }

        public async Task<List<int>> GetFavoriteCourseIdsAsync(int studentId, List<int> courseIds)
        {
            return await _context.Favorites
                .Where(f => f.StudentId == studentId && courseIds.Contains(f.CourseId))
                .Select(f => f.CourseId)
                .ToListAsync();
        }

        // Admin methods implementation
        public async Task<IEnumerable<Course>> GetAllCoursesForAdminAsync()
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                        .ThenInclude(l => l.Assignments)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Course> GetCourseByIdForAdminAsync(int courseId)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                        .ThenInclude(l => l.Assignments)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.Student)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<CourseStatisticsDto> GetCourseStatisticsAsync()
        {
            var totalCourses = await _context.Courses.CountAsync();
            var activeCourses = await _context.Courses.CountAsync(c => c.UpdatedAt != null);
            var inactiveCourses = await _context.Courses.CountAsync(c => c.UpdatedAt == null);
            var pendingCourses = await _context.Courses.CountAsync(c => c.UpdatedAt == null);
            var totalStudents = await _context.Users.CountAsync(u => u.RoleId == 1); // Assuming RoleId 1 is Student
            var totalTeachers = await _context.Users.CountAsync(u => u.RoleId == 3); // Assuming RoleId 3 is Teacher
            var totalReviews = await _context.Reviews.CountAsync();
            
            // Tính average rating an toàn
            var averageRating = totalReviews > 0 ? await _context.Reviews.AverageAsync(r => r.Rating) : 0;

            var coursesByCategoryData = await _context.Courses
                .Where(c => c.Category != null && !string.IsNullOrEmpty(c.Category.CategoryName))
                .GroupBy(c => c.Category.CategoryName)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync();
            var coursesByCategory = coursesByCategoryData.ToDictionary(x => x.Category, x => x.Count);

            var coursesByLevelData = await _context.Courses
                .Where(c => !string.IsNullOrEmpty(c.Level))
                .GroupBy(c => c.Level)
                .Select(g => new { Level = g.Key, Count = g.Count() })
                .ToListAsync();
            var coursesByLevel = coursesByLevelData.ToDictionary(x => x.Level, x => x.Count);

            var monthlyStats = await _context.Courses
                .Where(c => c.CreatedAt >= DateTime.UtcNow.AddMonths(-6))
                .GroupBy(c => new { c.CreatedAt.Value.Year, c.CreatedAt.Value.Month })
                .Select(g => new MonthlyCourseStats
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:00}",
                    NewCourses = g.Count(),
                    NewStudents = 0 // This would need to be calculated separately
                })
                .ToListAsync();

            return new CourseStatisticsDto
            {
                TotalCourses = totalCourses,
                ActiveCourses = activeCourses,
                InactiveCourses = inactiveCourses,
                PendingCourses = pendingCourses,
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                AverageRating = averageRating,
                TotalReviews = totalReviews,
                CoursesByCategory = coursesByCategory,
                CoursesByLevel = coursesByLevel,
                MonthlyStats = monthlyStats
            };
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Course>> GetPendingCoursesAsync()
        {
            // Assuming pending courses are those that haven't been updated yet
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                        .ThenInclude(l => l.Assignments)
                .Where(c => c.UpdatedAt == null)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByTeacherAsync(int teacherId)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                        .ThenInclude(l => l.Assignments)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .Where(c => c.TeacherId == teacherId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId)
        {
            return await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }
    }
} 