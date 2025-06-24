using AutoMapper;
using Sep490_Eduseen_BE.Dtos.Enrollment;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;
using System;
using System.Threading.Tasks;

namespace Sep490_Eduseen_BE.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository, ICourseRepository courseRepository, IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<(bool Success, string Message, EnrollmentDto Enrollment)> EnrollCourseAsync(int studentId, int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return (false, "Course not found.", null);
            }

            var isEnrolled = await _enrollmentRepository.IsEnrolledAsync(studentId, courseId);
            if (isEnrolled)
            {
                return (false, "You are already enrolled in this course.", null);
            }

            var newEnrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow,
                Status = "Enrolled" 
            };

            var createdEnrollment = await _enrollmentRepository.AddAsync(newEnrollment);
            var enrollmentDto = _mapper.Map<EnrollmentDto>(createdEnrollment);

            return (true, "Enrollment successful.", enrollmentDto);
        }
    }
} 