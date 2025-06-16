using System;

namespace Sep490_Eduseen_BE.Dtos.Enrollment
{
    public class EnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string Status { get; set; }
    }
} 