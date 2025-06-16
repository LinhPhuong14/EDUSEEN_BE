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
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
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
    }
} 