namespace Sep490_Eduseen_BE.Dtos.Course
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public string? Level { get; set; }
        public string? Cover { get; set; }
        public string TeacherName { get; set; }
        public string TeacherAvatarUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsFavorite { get; set; }
        public double Rating { get; set; }
        public double TotalTime { get; set; }
    }
} 