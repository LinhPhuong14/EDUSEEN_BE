using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Dtos;
using System.Security.Claims;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssignmentsController : ControllerBase
    {
        private readonly Sep490EduseenContext _context;

        public AssignmentsController(Sep490EduseenContext context)
        {
            _context = context;
        }

        [HttpGet("{assignmentId}")]
        public async Task<IActionResult> GetAssignmentDetail(int assignmentId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int studentId))
                return Unauthorized("Không thể xác định người dùng.");

            var assignment = await _context.Assignments
                .Include(a => a.Submissions)
                .Include(a => a.CreatedByNavigation)
                .Include(a => a.Lecture)
                    .ThenInclude(l => l.Section)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
                return NotFound("Không tìm thấy bài tập");

            var submission = assignment.Submissions
                .Where(s => s.StudentId == studentId)
                .OrderByDescending(s => s.AttemptNumber)
                .FirstOrDefault();

            var dto = new AssignmentDetailDto
            {
                AssignmentId = assignment.AssignmentId,
                Title = assignment.Title,
                Description = assignment.Description,
                DueDate = assignment.DueDate,
                CreatedByName = $"{assignment.CreatedByNavigation.FirstName} {assignment.CreatedByNavigation.LastName}",
                CreatedAt = assignment.CreatedAt,
                SubmissionStatus = submission == null ? "Chưa nộp" : (submission.Grade.HasValue ? "Đã chấm điểm" : "Đã nộp"),
                SubmittedAt = submission?.SubmittedAt,
                Grade = submission?.Grade,
                LectureId = assignment.LectureId,
                LectureTitle = assignment.Lecture.Title
            };

            return Ok(dto);
        }

    //[HttpGet("student/{studentId}/assignments")]
    //public async Task<IActionResult> GetAssignmentsForStudent(int studentId)
    //{
    //    var assignments = await _context.Assignments
    //        .Include(a => a.Submissions)
    //        .Include(a => a.Course)
    //        .OrderByDescending(a => a.DueDate)
    //        .Select(a => new
    //        {
    //            a.AssignmentId,
    //            a.Title,
    //            a.Description,
    //            a.DueDate,
    //            CourseTitle = a.Course.Title,
    //            IsSubmitted = a.Submissions.Any(s => s.StudentId == studentId)
    //        })
    //        .ToListAsync();

    //    return Ok(assignments);
    //}

    //[HttpPost("create")]
    //public async Task<IActionResult> CreateAssignment([FromBody] AssignmentDto dto)

    //{
    //    var assignment = new Assignment
    //    {
    //        Title = dto.Title,
    //        Description = dto.Description,
    //        DueDate = dto.DueDate,
    //        CreatedBy = dto.CreatedBy,
    //        CourseId = dto.CourseId,
    //        CreatedAt = DateTime.UtcNow
    //    };

    //    _context.Assignments.Add(assignment);
    //    await _context.SaveChangesAsync();

    //    // 🔔 Gửi notification đến các học sinh trong class có course này
    //    var studentIds = await _context.ClassCourses
    //        .Where(cc => cc.CourseId == dto.CourseId)
    //        .Join(_context.ClassStudents,
    //              cc => cc.ClassId,
    //              cs => cs.ClassId,
    //              (cc, cs) => cs.StudentId)
    //        .Distinct()
    //        .ToListAsync();

    //    var message = $"Bài tập mới: {assignment.Title} đã được giao cho bạn.";
    //    var notifications = studentIds.Select(sid => new Notification
    //    {
    //        UserId = sid,
    //        Message = message,
    //        CreatedAt = DateTime.UtcNow,
    //        IsRead = false
    //    }).ToList();

    //    _context.Notifications.AddRange(notifications);
    //    await _context.SaveChangesAsync();

    //    return Ok(new { message = "Tạo bài tập và gửi thông báo thành công" });
    //}



}
