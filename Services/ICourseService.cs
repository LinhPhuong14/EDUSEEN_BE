using Sep490_Eduseen_BE.Dtos.Course;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<IEnumerable<CourseDto>> SearchCoursesAsync(string courseName);
        Task<CourseDetailDto> GetCourseByIdAsync(int courseId);
        Task<IEnumerable<CourseDto>> GetMyCoursesAsync(int studentId);
        Task<(bool Success, string Message)> SaveFavoriteCourseAsync(int studentId, int courseId);
        Task<(bool Success, string Message, IEnumerable<LectureDto> Data)> GetCourseMaterialsAsync(int studentId, int courseId);
        Task<CourseProgressDto> GetCourseProgressAsync(int studentId, int courseId);
        Task<IEnumerable<CourseDto>> GetCompletedCoursesAsync(int studentId);
        Task<(bool Success, string Message)> RateCourseAsync(int studentId, int courseId, RateCourseRequestDto request);
    }
} 