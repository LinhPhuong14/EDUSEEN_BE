using Sep490_Eduseen_BE.Dtos.Submission;

namespace Sep490_Eduseen_BE.Services;

public interface IStudentSubmissionService
{
    Task<StudentSubmissionResponseDTO> SubmitAssignment(int studentId, StudentSubmitAssignmentDTO request);
} 