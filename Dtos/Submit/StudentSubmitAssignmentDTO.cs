using System.ComponentModel.DataAnnotations;

namespace Sep490_Eduseen_BE.Dtos.Submission;

public class StudentSubmitAssignmentDTO
{
    [Required]
    public int AssignmentId { get; set; }
    
    public string? SubmissionContent { get; set; }
    
    public List<SubmissionFileDTO>? Files { get; set; }
}

public class SubmissionFileDTO
{
    [Required]
    public string FileUrl { get; set; } = null!;
    
    public string? FileName { get; set; }
} 