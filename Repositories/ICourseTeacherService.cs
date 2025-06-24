using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Dtos.Teacher;

namespace Sep490_Eduseen_BE.Repositories
{
    public interface ICourseTeacherService
    {
        Task<CourseDto?> GetCourseAsync(int courseId, int teacherId);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, int teacherId);
        Task<bool> UpdateCourseAsync(int courseId, UpdateCourseDto dto, int teacherId);
        Task<bool> DeleteCourseAsync(int courseId, int teacherId);
        Task<CourseAnalysisDto> GetCourseAnalysisAsync(int courseId, int teacherId);
        Task<HomeworkAnalysisDto> GetHomeworkAnalysisAsync(int assignmentId, int teacherId);
    }
}
