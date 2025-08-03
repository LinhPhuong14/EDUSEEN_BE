using Sep490_Eduseen_BE.Dtos.Submission;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;

namespace Sep490_Eduseen_BE.Services;

public class StudentSubmissionService : IStudentSubmissionService
{
    private readonly IStudentSubmissionRepository _repository;

    public StudentSubmissionService(IStudentSubmissionRepository repository)
    {
        _repository = repository;
    }

    public async Task<StudentSubmissionResponseDTO> SubmitAssignment(int studentId, StudentSubmitAssignmentDTO request)
    {
        // Kiểm tra xem học sinh đã nộp bài cho assignment này chưa
        var existingSubmission = await _repository.GetLatestSubmissionByStudentAndAssignment(studentId, request.AssignmentId);
        
        // Lấy số lần nộp tiếp theo
        var nextAttemptNumber = await _repository.GetNextAttemptNumber(studentId, request.AssignmentId);
        
        // Tạo submission mới
        var newSubmission = new Submission
        {
            AssignmentId = request.AssignmentId,
            StudentId = studentId,
            AttemptNumber = nextAttemptNumber,
            SubmittedAt = DateTime.UtcNow,
            SubmissionContent = request.SubmissionContent
        };

        // Lưu submission
        var createdSubmission = await _repository.CreateSubmission(newSubmission);

        // Tạo submission files nếu có
        var submissionFiles = new List<SubmissionFile>();
        if (request.Files != null && request.Files.Any())
        {
            submissionFiles = request.Files.Select(file => new SubmissionFile
            {
                SubmissionId = createdSubmission.SubmissionId,
                FileUrl = file.FileUrl,
                FileName = file.FileName
            }).ToList();

            await _repository.CreateSubmissionFiles(submissionFiles);
        }

        // Tạo response
        var response = await _repository.GetSubmissionWithFiles(createdSubmission.SubmissionId);
        
        // Thêm message tùy theo trường hợp
        if (existingSubmission == null)
        {
            response.Message = "Bài tập đã được nộp thành công lần đầu tiên!";
            
            // Cập nhật UserLectureProgress khi nộp bài lần đầu
            await _repository.UpdateUserLectureProgress(studentId, request.AssignmentId);
        }
        else
        {
            response.Message = $"Bài tập đã được nộp lại thành công! (Lần nộp thứ {nextAttemptNumber})";
        }

        return response;
    }
} 