using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Dtos.Submission;
using Sep490_Eduseen_BE.Hubs;
using Sep490_Eduseen_BE.Models;
using System.IO;
using System.IO.Compression;

[ApiController]
[Route("api/[controller]")]
public class SubmissionController : ControllerBase
{
    private readonly Sep490EduseenContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IHubContext<SubmissionHub> _hubContext;

    public SubmissionController(
        Sep490EduseenContext context,
        IWebHostEnvironment env,
        IHubContext<SubmissionHub> hubContext)
    {
        _context = context;
        _env = env;
        _hubContext = hubContext;
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

    //[HttpDelete("{submissionId}")]
    //public async Task<IActionResult> DeleteSubmission(int submissionId)
    //{
    //    try
    //    {
    //        var submission = await _context.Submissions
    //            .Include(s => s.SubmissionFiles)
    //            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

    //        if (submission == null)
    //            return NotFound("Không tìm thấy bài nộp");

    //        foreach (var file in submission.SubmissionFiles ?? new List<SubmissionFile>())
    //        {
    //            var filePath = Path.Combine(_env.ContentRootPath, "Uploads", Path.GetFileName(file.FileUrl));
    //            if (System.IO.File.Exists(filePath))
    //            {
    //                System.IO.File.Delete(filePath);
    //            }
    //        }

    //        _context.Submissions.Remove(submission);
    //        await _context.SaveChangesAsync();

    //        // 🔔 Gửi sự kiện SignalR
    //        await _hubContext.Clients.All.SendAsync("SubmissionDeleted", new
    //        {
    //            submissionId = submission.SubmissionId,
    //            studentId = submission.StudentId,
    //            message = "Bài nộp đã bị xóa thành công"
    //        });

    //        return Ok(new { message = "Đã xóa bài nộp thành công" });
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"[DeleteSubmission] Error: {ex.Message}");
    //        return StatusCode(500, new { error = "Lỗi máy chủ", details = ex.Message });
    //    }
    //}

    [HttpDelete("{submissionId}")]
    public async Task<IActionResult> DeleteSubmission(int submissionId)
    {
        var submission = await _context.Submissions
            .Include(s => s.SubmissionFiles)
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

        if (submission == null)
            return NotFound("Không tìm thấy bài nộp.");

        if (submission.Grade.HasValue)
            return BadRequest("Không thể xóa bài đã được chấm điểm.");

        if (submission.Assignment?.DueDate != null && submission.SubmittedAt > submission.Assignment.DueDate)
            return BadRequest("Không thể xóa bài đã quá hạn nộp.");

        // Xóa file vật lý
        foreach (var file in submission.SubmissionFiles)
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Uploads", Path.GetFileName(file.FileUrl));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        // Xóa dữ liệu khỏi DB
        _context.Submissions.Remove(submission);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đã xóa bài nộp thành công." });
    }


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

    [HttpPut("edit")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> EditSubmission([FromForm] UploadRequestDTO request)
    {
        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(a => a.AssignmentId == request.AssignmentId);

        if (assignment == null)
            return NotFound("Không tìm thấy bài tập.");

        if (assignment.DueDate < DateTime.UtcNow)
            return BadRequest("Đã quá hạn nộp, không thể chỉnh sửa bài nộp.");

        var submission = await _context.Submissions
            .Include(s => s.SubmissionFiles)
            .FirstOrDefaultAsync(s =>
                s.AssignmentId == request.AssignmentId &&
                s.StudentId == request.StudentId);

        if (submission == null)
            return NotFound("Chưa có bài nộp để chỉnh sửa.");

        // Cập nhật nội dung bài nộp nếu có
        if (!string.IsNullOrWhiteSpace(request.SubmissionContent))
        {
            submission.SubmissionContent = request.SubmissionContent;
        }

        // Xóa file cũ nếu có file mới
        if (request.Files != null && request.Files.Count > 0)
        {
            foreach (var file in submission.SubmissionFiles)
            {
                var filePath = Path.Combine(_env.ContentRootPath, "Uploads", Path.GetFileName(file.FileUrl));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.SubmissionFiles.RemoveRange(submission.SubmissionFiles);
            submission.SubmissionFiles = new List<SubmissionFile>();

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
        }

        submission.SubmittedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Chỉnh sửa bài nộp thành công." });
    }

    [HttpPost("resubmit")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ResubmitHomework([FromForm] UploadRequestDTO request)
    {
        var assignment = await _context.Assignments.FindAsync(request.AssignmentId);
        if (assignment == null)
            return NotFound("Không tìm thấy bài tập.");

        // Optionally kiểm tra deadline:
        // if (assignment.DueDate < DateTime.UtcNow) return BadRequest("Đã quá hạn nộp.");

        // Tìm số lần đã nộp trước đó
        var previousAttempts = await _context.Submissions
            .Where(s => s.AssignmentId == request.AssignmentId && s.StudentId == request.StudentId)
            .ToListAsync();

        int newAttempt = previousAttempts.Count + 1;

        var newSubmission = new Submission
        {
            AssignmentId = request.AssignmentId,
            StudentId = request.StudentId,
            AttemptNumber = newAttempt,
            SubmittedAt = DateTime.UtcNow,
            SubmissionContent = request.SubmissionContent,
            SubmissionFiles = new List<SubmissionFile>()
        };

        string uploadPath = Path.Combine(_env.ContentRootPath, "Uploads");
        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        foreach (var file in request.Files)
        {
            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            newSubmission.SubmissionFiles.Add(new SubmissionFile
            {
                FileName = file.FileName,
                FileUrl = $"/uploads/{uniqueFileName}"
            });
        }

        _context.Submissions.Add(newSubmission);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = $"Nộp lại bài thành công (Lần {newAttempt})",
            submissionId = newSubmission.SubmissionId,
            attemptNumber = newSubmission.AttemptNumber
        });
    }

    [HttpPost("{submissionId}/feedback")]
    public async Task<IActionResult> SendFeedback(int submissionId, [FromBody] TeacherFeedbackDTO dto)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

        if (submission == null)
            return NotFound("Không tìm thấy bài nộp");

        submission.Feedback = dto.Feedback;
        submission.Grade = dto.Grade;

        // Gửi thông báo đến học sinh
        var notification = new Notification
        {
            UserId = submission.StudentId,
            Message = $"Bài nộp '{submission.SubmissionId}' đã được chấm điểm: {dto.Grade}/10",
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Đã gửi feedback và chấm điểm thành công." });
    }


    [HttpPost("student/{submissionId}/feedback")]
    public async Task<IActionResult> SubmitStudentFeedback(int submissionId, [FromBody] StudentFeedbackDTO dto)
    {
        var submission = await _context.Submissions.FindAsync(submissionId);
        if (submission == null)
            return NotFound("Không tìm thấy bài nộp");

        submission.Feedback = dto.Feedback;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đã gửi phản hồi đến giáo viên" });
    }

    [HttpGet("assignment/{assignmentId}/submissions")]
    public async Task<IActionResult> GetSubmissionsForAssignment(int assignmentId)
    {
        var submissions = await _context.Submissions
            .Include(s => s.Student)
            .Where(s => s.AssignmentId == assignmentId)
            .OrderByDescending(s => s.SubmittedAt)
            .Select(s => new SubmissionListItemDto
            {
                SubmissionId = s.SubmissionId,
                StudentId = s.StudentId,
                StudentName = s.Student.FirstName + " " + s.Student.LastName,
                AttemptNumber = s.AttemptNumber,
                SubmittedAt = s.SubmittedAt,
                Grade = s.Grade,
                Feedback = s.Feedback
            })
            .ToListAsync();

        return Ok(submissions);
    }


    [HttpGet("submission/{submissionId}/download")]
    public async Task<IActionResult> DownloadSubmissionFiles(int submissionId)
    {
        var submission = await _context.Submissions
            .Include(s => s.SubmissionFiles)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

        if (submission == null || submission.SubmissionFiles.Count == 0)
            return NotFound("Không tìm thấy bài nộp hoặc không có file.");

        var zipStream = new MemoryStream();

        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in submission.SubmissionFiles)
            {
                var filePath = Path.Combine(_env.ContentRootPath, "Uploads", Path.GetFileName(file.FileUrl));
                if (System.IO.File.Exists(filePath))
                {
                    var zipEntry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
                    using var originalFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    using var entryStream = zipEntry.Open();
                    await originalFileStream.CopyToAsync(entryStream);
                }
            }
        }

        zipStream.Position = 0; // reset stream before returning
        return File(zipStream, "application/zip", $"Submission_{submissionId}.zip");
    }



}
