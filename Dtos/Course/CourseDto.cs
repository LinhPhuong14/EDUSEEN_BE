namespace Sep490_Eduseen_BE.Dtos.Course
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public string? Level { get; set; }
        public string TeacherName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 