namespace Sep490_Eduseen_BE.Dtos;
public class EditSubmissionRequestDTO
{
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public string? SubmissionContent { get; set; }
    public List<IFormFile>? Files { get; set; }
}
