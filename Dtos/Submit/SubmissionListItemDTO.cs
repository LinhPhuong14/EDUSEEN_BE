namespace Sep490_Eduseen_BE.Dtos.Submission
{
    public class SubmissionListItemDto
    {
        public int SubmissionId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int AttemptNumber { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public decimal? Grade { get; set; }
        public string? Feedback { get; set; }
    }
}
