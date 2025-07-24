using System;

namespace Sep490_Eduseen_BE.Dtos.Review
{
    public class ReviewDto
    {
        public string UserName { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public string? CourseDescription { get; set; }
        public string? UserAvatarUrl { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}