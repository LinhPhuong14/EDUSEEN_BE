using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Repositories.impl
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly Sep490EduseenContext _context;

        public EnrollmentRepository(Sep490EduseenContext context)
        {
            _context = context;
        }

        public async Task<bool> IsEnrolledAsync(int studentId, int courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task<Enrollment> AddAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }
    }
} 