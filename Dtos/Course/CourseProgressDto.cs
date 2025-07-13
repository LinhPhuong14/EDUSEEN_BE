namespace Sep490_Eduseen_BE.Dtos.Course
{
    public class CourseProgressDto
    {
        public int CourseId { get; set; }
        public int TotalLectures { get; set; }
        public int CompletedLectures { get; set; }
        public double ProgressPercentage { get; set; }
    }
} 