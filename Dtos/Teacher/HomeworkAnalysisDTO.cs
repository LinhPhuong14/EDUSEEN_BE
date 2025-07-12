namespace Sep490_Eduseen_BE.Dtos.Teacher
{
    public class HomeworkAnalysisDto
    {
        public int AssignmentId { get; set; }
        public int TotalAssigned { get; set; }
        public int TotalSubmitted { get; set; }
        public double CompletionRate { get; set; }
        public int LateSubmissionCount { get; set; }
        public double LateSubmissionRate { get; set; }
        public double? AverageGrade { get; set; }
        public Dictionary<string, int>? GradeDistribution { get; set; }
        public int GradedCount { get; set; }
        public List<StudentInfoDto> NotSubmittedStudents { get; set; } = new();
    }

    public class StudentInfoDto
    {
        public int StudentId { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
    }


}
