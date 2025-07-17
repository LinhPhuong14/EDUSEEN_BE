using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Dtos.Teacher;

namespace Sep490_Eduseen_BE.Repositories
{
    public interface ICourseTeacherService
    {
        Task<CourseDTO?> GetCourseAsync(int courseId, int teacherId);
        Task<CourseDTO> CreateCourseAsync(CreateCourseDTO dto, int teacherId);
        Task<bool> UpdateCourseAsync(int courseId, UpdateCourseDTO dto, int teacherId);
        Task<bool> DeleteCourseAsync(int courseId, int teacherId);
        Task<CourseAnalysisDTO> GetCourseAnalysisAsync(int courseId, int teacherId);
        //Task<HomeworkAnalysisDto> GetHomeworkAnalysisAsync(int assignmentId, int teacherId);
        Task<IEnumerable<CourseDTO>> GetCoursesAsync(int teacherId);
    }
}
