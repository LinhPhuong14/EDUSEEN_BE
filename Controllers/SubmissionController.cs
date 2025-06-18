using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Dtos;
using System.IO;

[ApiController]
[Route("api/[controller]")]
public class SubmissionController : ControllerBase
{
    private readonly Sep490EduseenContext _context;
    private readonly IWebHostEnvironment _env;

    public SubmissionController(Sep490EduseenContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadSubmission([FromForm] UploadRequestDTO request)
    {
        var previousAttempts = await _context.Submissions
            .Where(s => s.AssignmentId == request.AssignmentId && s.StudentId == request.StudentId)
            .ToListAsync();

        int attemptNumber = previousAttempts.Count + 1;

        var submission = new Submission
        {
            AssignmentId = request.AssignmentId,
            StudentId = request.StudentId,
            AttemptNumber = attemptNumber,
            SubmittedAt = DateTime.UtcNow,
            SubmissionContent = request.SubmissionContent,
            Grade = null,
            Feedback = null,
            SubmissionFiles = new List<SubmissionFile>()
        };

        string uploadPath = Path.Combine(_env.ContentRootPath, "Uploads");
        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        foreach (var file in request.Files)
        {
            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(uploadPath, uniqueFileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            submission.SubmissionFiles.Add(new SubmissionFile
            {
                FileName = file.FileName,
                FileUrl = $"/uploads/{uniqueFileName}"
            });
        }

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Bài tập đã được nộp thành công",
            submissionId = submission.SubmissionId,
            attemptNumber = submission.AttemptNumber
        });
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetSubmissionStatus(int assignmentId, int studentId)
    {
        var latestSubmission = await _context.Submissions
            .Where(s => s.AssignmentId == assignmentId && s.StudentId == studentId)
            .OrderByDescending(s => s.AttemptNumber)
            .FirstOrDefaultAsync();

        if (latestSubmission == null)
        {
            return Ok(new
            {
                Status = "Chưa nộp",
                Grade = (decimal?)null,
                SubmittedAt = (DateTime?)null
            });
        }

        var status = latestSubmission.Grade.HasValue ? "Đã chấm điểm" : "Đã nộp";

        return Ok(new
        {
            Status = status,
            Grade = latestSubmission.Grade,
            SubmittedAt = latestSubmission.SubmittedAt
        });
    }

    [HttpDelete("/api/submissions/{submissionId}")]
    public async Task<IActionResult> DeleteSubmission(int submissionId)
    {
        var submission = await _context.Submissions
            .Include(s => s.SubmissionFiles)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

        if (submission == null)
            return NotFound("Không tìm thấy bài nộp");

        // delete file 
        foreach (var file in submission.SubmissionFiles)
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Uploads", Path.GetFileName(file.FileUrl));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.Submissions.Remove(submission);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đã xóa bài nộp thành công" });
    }

    //  NEW: Get assignment detail by student & assignment
    [HttpGet("assignment-detail/{assignmentId}/student/{studentId}")]
    public async Task<IActionResult> GetAssignmentDetailForStudent(int assignmentId, int studentId)
    {
        var assignment = await _context.Assignments
            .Include(a => a.CreatedByNavigation)
            .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

        if (assignment == null)
            return NotFound("Không tìm thấy bài tập");

        var latestSubmission = await _context.Submissions
            .Where(s => s.AssignmentId == assignmentId && s.StudentId == studentId)
            .OrderByDescending(s => s.AttemptNumber)
            .FirstOrDefaultAsync();

        var dto = new AssignmentDetailDto
        {
            AssignmentId = assignment.AssignmentId,
            Title = assignment.Title,
            Description = assignment.Description,
            DueDate = assignment.DueDate,
            CreatedByName = $"{assignment.CreatedByNavigation.FirstName} {assignment.CreatedByNavigation.LastName}",
            CreatedAt = assignment.CreatedAt,
            SubmissionStatus = latestSubmission == null ? "Chưa nộp" : (latestSubmission.Grade.HasValue ? "Đã chấm điểm" : "Đã nộp"),
            SubmittedAt = latestSubmission?.SubmittedAt,
            Grade = latestSubmission?.Grade
        };

        return Ok(dto);
    }
}
