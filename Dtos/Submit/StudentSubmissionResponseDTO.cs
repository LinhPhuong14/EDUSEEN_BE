namespace Sep490_Eduseen_BE.Dtos.Submission;

public class StudentSubmissionResponseDTO
{
    public int SubmissionId { get; set; }
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string? SubmissionContent { get; set; }
    public List<SubmissionFileResponseDTO> Files { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class SubmissionFileResponseDTO
{
    public int FileId { get; set; }
    public string FileUrl { get; set; } = null!;
    public string? FileName { get; set; }
} 