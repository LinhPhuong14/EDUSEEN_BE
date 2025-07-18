namespace Sep490_Eduseen_BE.Dtos.Teacher
{
    public class AssignmentOverviewDto
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SectionTitle { get; set; } = string.Empty;
        public string LectureTitle { get; set; } = string.Empty;
        public int TotalAssigned { get; set; }
        public int TotalSubmitted { get; set; }
        public double? AverageGrade { get; set; }
    }
} 