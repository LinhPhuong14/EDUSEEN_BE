namespace Sep490_Eduseen_BE.Dtos.Course
{
    public class LectureDto
    {
        public int LectureId { get; set; }
        public string Title { get; set; }
        public int? Duration { get; set; } // Duration in minutes or seconds, depends on the business logic
        public string? ContentUrl { get; set; }
    }
} 