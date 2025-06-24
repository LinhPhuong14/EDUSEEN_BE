using Sep490_Eduseen_BE.Dtos.Enrollment;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Services
{
    public interface IEnrollmentService
    {
        Task<(bool Success, string Message, EnrollmentDto Enrollment)> EnrollCourseAsync(int studentId, int courseId);
    }
} 