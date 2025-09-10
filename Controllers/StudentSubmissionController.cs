using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Dtos;
using Sep490_Eduseen_BE.Dtos.Submission;
using Sep490_Eduseen_BE.Hubs;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Services;
using System.IO;
using System.IO.Compression;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class StudentSubmissionController : ControllerBase
{
    private readonly Sep490EduseenContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IHubContext<SubmissionHub> _hubContext;
    private readonly IStudentSubmissionService _studentSubmissionService;

    public StudentSubmissionController(
        Sep490EduseenContext context,
        IWebHostEnvironment env,
        IHubContext<SubmissionHub> hubContext,
        IStudentSubmissionService studentSubmissionService)
    {
        _context = context;
        _env = env;
        _hubContext = hubContext;
        _studentSubmissionService = studentSubmissionService;
    }

    /// <summary>
    /// Học sinh nộp bài tập
    /// </summary>
    /// <param name="assignmentId">ID của bài tập</param>
    /// <param name="request">Thông tin bài nộp</param>
    /// <returns>Thông tin bài nộp đã tạo</returns>
    [HttpPut("{assignmentId}")]
    public async Task<ActionResult<StudentSubmissionResponseDTO>> SubmitAssignment(
        int assignmentId, 
        [FromBody] StudentSubmitAssignmentDTO request)
    {
        try
        {
            // Validate assignmentId trong request phải khớp với assignmentId trong URL
            if (request.AssignmentId != assignmentId)
            {
                return BadRequest("AssignmentId trong request không khớp với AssignmentId trong URL");
            }

            // Lấy studentId từ token (giả sử có middleware xác thực)
            // Trong thực tế, bạn sẽ lấy từ JWT token hoặc session
            var studentId = GetCurrentUserId(); // Implement method này theo cách xác thực của bạn

            if (studentId == 0)
            {
                return Unauthorized("Không thể xác định học sinh");
            }

            // Kiểm tra xem assignment có tồn tại không
            var assignment = await _context.Assignments.FindAsync(assignmentId);
            if (assignment == null)
            {
                return NotFound("Không tìm thấy bài tập");
            }

            // Gọi service để xử lý logic nộp bài
            var result = await _studentSubmissionService.SubmitAssignment(studentId, request);

            // Gửi notification qua SignalR nếu cần
            await _hubContext.Clients.All.SendAsync("NewSubmission", new
            {
                AssignmentId = assignmentId,
                StudentId = studentId,
                SubmissionId = result.SubmissionId,
                AttemptNumber = result.AttemptNumber
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi nộp bài tập", error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy thông tin chi tiết bài nộp của học sinh
    /// </summary>
    /// <param name="assignmentId">ID của bài tập</param>
    /// <returns>Thông tin bài nộp mới nhất</returns>
    [HttpGet("{assignmentId}")]
    public async Task<ActionResult<StudentSubmissionResponseDTO>> GetSubmissionDetail(int assignmentId)
    {
        try
        {
            var studentId = GetCurrentUserId();
            if (studentId == 0)
            {
                return Unauthorized("Không thể xác định học sinh");
            }

            var submission = await _context.Submissions
                .Include(s => s.SubmissionFiles)
                .Where(s => s.StudentId == studentId && s.AssignmentId == assignmentId)
                .OrderByDescending(s => s.AttemptNumber)
                .FirstOrDefaultAsync();

            if (submission == null)
            {
                return NotFound("Chưa có bài nộp cho bài tập này");
            }

            var result = new StudentSubmissionResponseDTO
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
                }).ToList(),
                Message = "Lấy thông tin bài nộp thành công"
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy thông tin bài nộp", error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách tất cả bài nộp của học sinh cho một bài tập
    /// </summary>
    /// <param name="assignmentId">ID của bài tập</param>
    /// <returns>Danh sách các lần nộp bài</returns>
    [HttpGet("{assignmentId}/history")]
    public async Task<ActionResult<List<StudentSubmissionResponseDTO>>> GetSubmissionHistory(int assignmentId)
    {
        try
        {
            var studentId = GetCurrentUserId();
            if (studentId == 0)
            {
                return Unauthorized("Không thể xác định học sinh");
            }

            var submissions = await _context.Submissions
                .Include(s => s.SubmissionFiles)
                .Where(s => s.StudentId == studentId && s.AssignmentId == assignmentId)
                .OrderByDescending(s => s.AttemptNumber)
                .ToListAsync();

            var result = submissions.Select(s => new StudentSubmissionResponseDTO
            {
                SubmissionId = s.SubmissionId,
                AssignmentId = s.AssignmentId,
                StudentId = s.StudentId,
                AttemptNumber = s.AttemptNumber,
                SubmittedAt = s.SubmittedAt ?? DateTime.UtcNow,
                SubmissionContent = s.SubmissionContent,
                Grade = s.Grade,
                Feedback = s.Feedback,
                Files = s.SubmissionFiles.Select(f => new SubmissionFileResponseDTO
                {
                    FileId = f.FileId,
                    FileUrl = f.FileUrl,
                    FileName = f.FileName
                }).ToList(),
                Message = $"Lần nộp thứ {s.AttemptNumber}"
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy lịch sử bài nộp", error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy ID của user hiện tại từ token/session
    /// Implement method này theo cách xác thực của bạn
    /// </summary>
    /// <returns>User ID</returns>
    private int GetCurrentUserId()
    {
        var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(studentIdString, out var studentId))
        {
            return 0;
        }

        return studentId;
    }
}
