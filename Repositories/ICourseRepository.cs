using Sep490_Eduseen_BE.Models;
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
    }
} 