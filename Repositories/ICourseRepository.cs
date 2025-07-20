using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Dtos.Course;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Repositories
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<IEnumerable<Course>> SearchCoursesAsync(string courseName);
        Task<Course> GetCourseByIdAsync(int courseId);
        Task<IEnumerable<Course>> GetEnrolledCoursesByStudentIdAsync(int studentId);
        Task<Favorite> GetFavoriteAsync(int studentId, int courseId);
        Task AddFavoriteAsync(Favorite favorite);
        Task<bool> IsUserEnrolledAsync(int studentId, int courseId);
        Task<IEnumerable<Lecture>> GetLecturesByCourseIdAsync(int courseId);
        Task<int> GetCompletedLecturesCountAsync(int studentId, int courseId);
        Task<Review> GetReviewAsync(int studentId, int courseId);
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task<IEnumerable<Review>> GetTopReviewsAsync(int count = 3);

        // Admin methods
        Task<IEnumerable<Course>> GetAllCoursesForAdminAsync();
        Task<Course> GetCourseByIdForAdminAsync(int courseId);
        Task<CourseStatisticsDto> GetCourseStatisticsAsync();
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int courseId);
        Task<IEnumerable<Course>> GetPendingCoursesAsync();
        Task<IEnumerable<Course>> GetCoursesByTeacherAsync(int teacherId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId);
    }
} 