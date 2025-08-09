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

        [HttpGet("student/assignments")]
        public async Task<IActionResult> GetAssignmentsForStudent()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int studentId))
                return Unauthorized("Không thể xác định người dùng.");

            var assignments = await _context.Assignments
                .Include(a => a.Submissions)
                .Include(a => a.Lecture)
                .OrderByDescending(a => a.DueDate)
                .Select(a => new
                {
                    a.AssignmentId,
                    a.Title,
                    a.Description,
                    a.DueDate,
                    LectureTitle = a.Lecture.Title,
                    IsSubmitted = a.Submissions.Any(s => s.StudentId == studentId)
                })
                .ToListAsync();

            return Ok(assignments);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAssignment([FromBody] AssignmentDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Không thể xác định người dùng.");

            var lecture = await _context.Lectures
                .Include(l => l.Section)
                .FirstOrDefaultAsync(l => l.LectureId == dto.LectureId);

            if (lecture == null)
                return NotFound("Không tìm thấy bài giảng để gán bài tập.");

            // Kiểm tra xem lecture đã có assignment hay chưa
            var oldAssignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.LectureId == dto.LectureId);

            if (oldAssignment != null)
            {
                // Ghi đè: cập nhật lại assignment cũ
                oldAssignment.Title = dto.Title;
                oldAssignment.Description = dto.Description;
                oldAssignment.DueDate = dto.DueDate;
                oldAssignment.CreatedBy = userId;
                oldAssignment.CreatedAt = DateTime.UtcNow;

                _context.Assignments.Update(oldAssignment);
                await _context.SaveChangesAsync();

                // Gửi thông báo đến học sinh thuộc course qua section
                var courseId = lecture.Section.CourseId;
                var studentIds = await _context.Enrollments
                    .Where(e => e.CourseId == courseId)
                    .Select(e => e.StudentId)
                    .Distinct()
                    .ToListAsync();

                var message = $"Bài tập mới: {oldAssignment.Title} đã được cập nhật cho bạn.";
                var notifications = studentIds.Select(sid => new Notification
                {
                    UserId = sid,
                    Message = message,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                }).ToList();

                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật bài tập thành công (đã ghi đè bài tập cũ)" });
            }
            else
            {
                // Chưa có assignment, tạo mới như cũ
                var assignment = new Assignment
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    DueDate = dto.DueDate,
                    CreatedBy = userId,
                    LectureId = dto.LectureId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();

                // Gửi thông báo đến học sinh thuộc course qua section
                var courseId = lecture.Section.CourseId;
                var studentIds = await _context.Enrollments
                    .Where(e => e.CourseId == courseId)
                    .Select(e => e.StudentId)
                    .Distinct()
                    .ToListAsync();

                var message = $"Bài tập mới: {assignment.Title} đã được giao cho bạn.";
                var notifications = studentIds.Select(sid => new Notification
                {
                    UserId = sid,
                    Message = message,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                }).ToList();

                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tạo bài tập và gửi thông báo thành công" });
            }
        }

        [HttpPut("{assignmentId}")]
        public async Task<IActionResult> UpdateAssignment(int assignmentId, [FromBody] UpdateAssignmentDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Không thể xác định người dùng.");

            var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
            if (assignment == null)
                return NotFound("Không tìm thấy bài tập.");

            // Chỉ cho phép người tạo bài tập hoặc teacher (role check có thể bổ sung sau) chỉnh sửa
            if (assignment.CreatedBy != userId)
            {
                // TODO: kiểm tra quyền teacher nếu cần
                return Forbid("Bạn không có quyền chỉnh sửa bài tập này.");
            }

            // Kiểm tra lecture hợp lệ nếu thay đổi
            if (assignment.LectureId != dto.LectureId)
            {
                var lecture = await _context.Lectures.FindAsync(dto.LectureId);
                if (lecture == null)
                    return NotFound("Không tìm thấy bài giảng.");

                assignment.LectureId = dto.LectureId;
            }

            assignment.Title = dto.Title;
            assignment.Description = dto.Description;
            assignment.DueDate = dto.DueDate;

            _context.Assignments.Update(assignment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật bài tập thành công." });
        }

        [HttpDelete("{assignmentId}")]
        public async Task<IActionResult> DeleteAssignment(int assignmentId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Không thể xác định người dùng.");

            var assignment = await _context.Assignments
                .Include(a => a.Submissions)
                    .ThenInclude(s => s.SubmissionFiles)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
                return NotFound("Không tìm thấy bài tập.");

            // Chỉ cho phép người tạo bài tập xóa
            if (assignment.CreatedBy != userId)
            {
                return Forbid("Bạn không có quyền xóa bài tập này.");
            }

            try
            {
                // Xóa tất cả submission files trước
                foreach (var submission in assignment.Submissions)
                {
                    _context.SubmissionFiles.RemoveRange(submission.SubmissionFiles);
                }

                // Xóa tất cả submissions
                _context.Submissions.RemoveRange(assignment.Submissions);

                // Xóa assignment
                _context.Assignments.Remove(assignment);

                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa bài tập thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa bài tập: " + ex.Message });
            }
        }
    }
}
