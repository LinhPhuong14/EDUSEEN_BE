namespace Sep490_Eduseen_BE.Dtos.Teacher
{
    using Sep490_Eduseen_BE.Dtos.Submission;
    public class AssignmentSubmissionsDto
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public IEnumerable<SubmissionListItemDto> Submissions { get; set; } = new List<SubmissionListItemDto>();
        public List<StudentInfoDto> NotSubmittedStudents { get; set; } = new();
    }
} 