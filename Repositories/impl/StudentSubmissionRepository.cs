using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Dtos.Submission;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Repositories.impl;

public class StudentSubmissionRepository : IStudentSubmissionRepository
{
    private readonly Sep490EduseenContext _context;

    public StudentSubmissionRepository(Sep490EduseenContext context)
    {
        _context = context;
    }

    public async Task<Submission?> GetLatestSubmissionByStudentAndAssignment(int studentId, int assignmentId)
    {
        return await _context.Submissions
            .Where(s => s.StudentId == studentId && s.AssignmentId == assignmentId)
            .OrderByDescending(s => s.AttemptNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetNextAttemptNumber(int studentId, int assignmentId)
    {
        var latestSubmission = await GetLatestSubmissionByStudentAndAssignment(studentId, assignmentId);
        return latestSubmission?.AttemptNumber + 1 ?? 1;
    }

    public async Task<Submission> CreateSubmission(Submission submission)
    {
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();
        return submission;
    }

    public async Task<List<SubmissionFile>> CreateSubmissionFiles(List<SubmissionFile> submissionFiles)
    {
        _context.SubmissionFiles.AddRange(submissionFiles);
        await _context.SaveChangesAsync();
        return submissionFiles;
    }

    public async Task<StudentSubmissionResponseDTO> GetSubmissionWithFiles(int submissionId)
    {
        var submission = await _context.Submissions
            .Include(s => s.SubmissionFiles)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

        if (submission == null)
            throw new ArgumentException("Submission not found");

        return new StudentSubmissionResponseDTO
        {
            SubmissionId = submission.SubmissionId,
            AssignmentId = submission.AssignmentId,
            StudentId = submission.StudentId,
            AttemptNumber = submission.AttemptNumber,
            SubmittedAt = submission.SubmittedAt ?? DateTime.UtcNow,
            SubmissionContent = submission.SubmissionContent,
            Grade = submission.Grade,
            Feedback = submission.Feedback,
            Files = submission.SubmissionFiles.Select(f => new SubmissionFileResponseDTO
            {
                FileId = f.FileId,
                FileUrl = f.FileUrl,
                FileName = f.FileName
            }).ToList()
        };
    }

    public async Task UpdateUserLectureProgress(int studentId, int assignmentId)
    {
        try
        {
            // Lấy thông tin assignment để biết LectureId
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
            {
                return; // Không tìm thấy assignment
            }

            // Kiểm tra xem đã có progress cho lecture này chưa
            var existingProgress = await _context.UserLectureProgresses
                .FirstOrDefaultAsync(p => p.UserId == studentId && p.LectureId == assignment.LectureId);

            if (existingProgress != null)
            {
                // Cập nhật progress hiện tại
                existingProgress.IsCompleted = true;
                existingProgress.LastAccessed = DateTime.UtcNow;
            }
            else
            {
                // Tạo mới progress
                var newProgress = new UserLectureProgress
                {
                    UserId = studentId,
                    LectureId = assignment.LectureId,
                    IsCompleted = true,
                    LastAccessed = DateTime.UtcNow
                };

                _context.UserLectureProgresses.Add(newProgress);
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log lỗi nhưng không throw exception để không ảnh hưởng đến việc nộp bài
            // Trong thực tế, bạn có thể sử dụng ILogger để log
            Console.WriteLine($"Lỗi khi cập nhật UserLectureProgress: {ex.Message}");
        }
    }
} 