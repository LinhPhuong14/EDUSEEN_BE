using Sep490_Eduseen_BE.Models;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<bool> IsEnrolledAsync(int studentId, int courseId);
        Task<Enrollment> AddAsync(Enrollment enrollment);
    }
} 