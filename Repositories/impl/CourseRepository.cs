using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
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
    }
} 