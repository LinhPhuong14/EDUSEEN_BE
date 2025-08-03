using Sep490_Eduseen_BE.Dtos.Submission;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Repositories;

public interface IStudentSubmissionRepository
{
    Task<Submission?> GetLatestSubmissionByStudentAndAssignment(int studentId, int assignmentId);
    Task<int> GetNextAttemptNumber(int studentId, int assignmentId);
    Task<Submission> CreateSubmission(Submission submission);
    Task<List<SubmissionFile>> CreateSubmissionFiles(List<SubmissionFile> submissionFiles);
    Task<StudentSubmissionResponseDTO> GetSubmissionWithFiles(int submissionId);
    
    // Thêm method để xử lý UserLectureProgress
    Task UpdateUserLectureProgress(int studentId, int assignmentId);
} 