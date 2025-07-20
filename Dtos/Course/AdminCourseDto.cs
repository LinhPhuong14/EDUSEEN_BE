using System;
using System.Collections.Generic;
using Sep490_Eduseen_BE.Dtos.Review;
using Sep490_Eduseen_BE.Dtos.Enrollment;

namespace Sep490_Eduseen_BE.Dtos.Course
{
    public class AdminCourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public int? CategoryId { get; set; }
        public string? Level { get; set; }
        public string TeacherName { get; set; }
        public int TeacherId { get; set; }
        public string TeacherEmail { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int StudentCount { get; set; }
        public int SectionCount { get; set; }
        public int LectureCount { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string Status { get; set; } // "Active", "Inactive", "Pending"
        public string? ThumbnailUrl { get; set; }
    }

    public class AdminCourseDetailDto : AdminCourseDto
    {
        public List<SectionDto> Sections { get; set; } = new List<SectionDto>();
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
        public List<EnrollmentDto> Enrollments { get; set; } = new List<EnrollmentDto>();
    }

    public class UpdateCourseStatusDto
    {
        public bool IsActive { get; set; }
    }

    public class CourseStatisticsDto
    {
        public int TotalCourses { get; set; }
        public int ActiveCourses { get; set; }
        public int InactiveCourses { get; set; }
        public int PendingCourses { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<string, int> CoursesByCategory { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CoursesByLevel { get; set; } = new Dictionary<string, int>();
        public List<MonthlyCourseStats> MonthlyStats { get; set; } = new List<MonthlyCourseStats>();
    }

    public class MonthlyCourseStats
    {
        public string Month { get; set; }
        public int NewCourses { get; set; }
        public int NewStudents { get; set; }
    }
} 