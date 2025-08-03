using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;
using Sep490_Eduseen_BE.Services;
using System.Security.Claims;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/schedule")]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly Sep490EduseenContext _context;
        private readonly IEmailService _emailService;

        public ScheduleController(Sep490EduseenContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("set")]
        public async Task<IActionResult> SetSchedule([FromBody] ScheduleRequestDTO dto)
        {
            var senderEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(senderEmail))
                return Unauthorized(new { message = "Không thể xác định người dùng." });

            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Email == senderEmail);
            var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.ReceiverEmail);

            if (sender == null || receiver == null)
                return NotFound(new { message = "Người gửi hoặc người nhận không tồn tại." });

            if (sender.UserId == receiver.UserId)
                return BadRequest(new { message = "Không thể đặt lịch với chính mình." });

            if (dto.ScheduledTime < DateTime.UtcNow)
                return BadRequest(new { message = "Thời gian đặt lịch không hợp lệ." });

            var schedule = new Schedule
            {
                TeacherId = sender.UserId,
                StudentId = receiver.UserId,
                ScheduledTime = dto.ScheduledTime,
                Duration = dto.Duration,
                Status = "Scheduled",
                CourseId = dto.CourseId // Có thể null
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            var subjectToReceiver = "Bạn có một lịch gọi video mới!";
            var courseInfo = dto.CourseId.HasValue ? $"<p><strong>Khóa học:</strong> [Khóa học ID: {dto.CourseId}]</p>" : "";
            var bodyToReceiver = $@"
                <h3>Lịch học mới từ EDUSEEN</h3>
                <p><strong>Thời gian:</strong> {dto.ScheduledTime:HH:mm dd/MM/yyyy}</p>
                <p><strong>Người đặt lịch:</strong> {sender.Username} ({senderEmail})</p>
                {courseInfo}
                <p>Vui lòng truy cập hệ thống để xác nhận cuộc gọi.</p>
            ";

            try
            {
                await _emailService.SendEmailAsync(receiver.Email, subjectToReceiver, bodyToReceiver);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email đến người nhận: {ex.Message}");
            }

            var subjectToSender = "Xác nhận đặt lịch gọi thành công";
            var courseInfoSender = dto.CourseId.HasValue ? $"<p><strong>Khóa học:</strong> [Khóa học ID: {dto.CourseId}]</p>" : "";
            var bodyToSender = $@"
                <h3>Đặt lịch thành công!</h3>
                <p>Bạn đã đặt một lịch học với người dùng <strong>{receiver.Username} ({receiver.Email})</strong>.</p>
                <p><strong>Thời gian:</strong> {dto.ScheduledTime:HH:mm dd/MM/yyyy}</p>
                {courseInfoSender}
                <p>Hệ thống EDUSEEN đã gửi thông báo đến người nhận.</p>
            ";

            try
            {
                await _emailService.SendEmailAsync(sender.Email, subjectToSender, bodyToSender);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email xác nhận cho người đặt lịch: {ex.Message}");
            }

            return Ok(new { message = "Đặt lịch thành công và đã gửi email xác nhận đến cả hai bên." });
        }

        [HttpGet("mySchedules")]
        public async Task<IActionResult> GetMySchedules()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Không xác định được người dùng.");

            var schedules = await _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Teacher)
                .Include(s => s.Student)
                .Where(s => s.TeacherId == userId || s.StudentId == userId)
                .OrderByDescending(s => s.ScheduledTime)
                .Select(s => new
                {
                    s.ScheduleId,
                    s.ScheduledTime,
                    s.Duration,
                    s.Status,
                    CourseTitle = s.Course != null ? s.Course.Title : "Tư vấn",
                    PartnerName = s.TeacherId == userId
                        ? $"{s.Student.FirstName} {s.Student.LastName}"
                        : $"{s.Teacher.FirstName} {s.Teacher.LastName}",
                    Role = s.TeacherId == userId ? "Teacher" : "Student"
                })
                .ToListAsync();

            return Ok(schedules);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelSchedule(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new { message = "Không xác định được người dùng." });

            var schedule = await _context.Schedules
                .Include(s => s.Teacher)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);

            if (schedule == null)
                return NotFound(new { message = "Không tìm thấy lịch gọi." });

            // Kiểm tra người dùng hiện tại có phải là 1 trong 2 người tham gia lịch không
            if (schedule.Teacher.Email != userEmail && schedule.Student.Email != userEmail)
                return Forbid("Bạn không có quyền huỷ lịch này.");

            // Lưu thông tin trước khi xoá để gửi mail
            var receiver = schedule.Teacher.Email == userEmail ? schedule.Student : schedule.Teacher;
            var receiverEmail = receiver.Email;
            var receiverName = $"{receiver.FirstName} {receiver.LastName}";

            var subject = "Lịch gọi video đã bị huỷ";
            var timeInfo = schedule.ScheduledTime.ToString("HH:mm dd/MM/yyyy");

            var bodyToReceiver = $@"
        <h3>Lịch gọi đã bị huỷ</h3>
        <p>Lịch gọi video lúc <strong>{timeInfo}</strong> với người dùng <strong>{userEmail}</strong> đã bị huỷ.</p>
    ";

            var bodyToSender = $@"
        <h3>Huỷ lịch thành công</h3>
        <p>Bạn đã huỷ lịch gọi video lúc <strong>{timeInfo}</strong> với người dùng <strong>{receiverName} ({receiverEmail})</strong>.</p>
    ";

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            try
            {
                await _emailService.SendEmailAsync(userEmail, subject, bodyToSender);
                await _emailService.SendEmailAsync(receiverEmail, subject, bodyToReceiver);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email khi huỷ lịch: {ex.Message}");
            }

            return Ok(new { message = "Huỷ lịch thành công và đã gửi email thông báo." });
        }

        [HttpPut("update-my-schedule/{id}")]
        public async Task<IActionResult> UpdateMySchedule(int id, [FromBody] ScheduleRequestDTO dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new { message = "Không thể xác định người dùng." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return Unauthorized(new { message = "Người dùng không tồn tại trong hệ thống." });

            var schedule = await _context.Schedules
                .Include(s => s.Teacher)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);

            if (schedule == null)
                return NotFound(new { message = "Không tìm thấy lịch gọi." });

            // Kiểm tra quyền sở hữu lịch
            if (schedule.TeacherId != user.UserId && schedule.StudentId != user.UserId)
                return Forbid("Bạn không có quyền cập nhật lịch này.");

            if (dto.ScheduledTime < DateTime.UtcNow)
                return BadRequest(new { message = "Thời gian đặt lịch không hợp lệ." });

            // Cập nhật dữ liệu
            schedule.ScheduledTime = dto.ScheduledTime;
            schedule.Duration = dto.Duration;
            schedule.CourseId = dto.CourseId; // Có thể null
            schedule.Status = "Rescheduled";

            await _context.SaveChangesAsync();

            // Xác định người nhận còn lại
            var receiver = schedule.TeacherId == user.UserId ? schedule.Student : schedule.Teacher;

            var timeStr = dto.ScheduledTime.ToString("HH:mm dd/MM/yyyy");

            var subjectToReceiver = "Cập nhật lịch gọi từ EDUSEEN";
            var courseInfoUpdate = dto.CourseId.HasValue ? $"<p><strong>Khóa học:</strong> [Khóa học ID: {dto.CourseId}]</p>" : "";
            var bodyToReceiver = $@"
        <h3>Lịch học đã được cập nhật!</h3>
        <p><strong>Người cập nhật:</strong> {user.Username} ({user.Email})</p>
        <p><strong>Thời gian mới:</strong> {timeStr}</p>
        {courseInfoUpdate}
        <p>Vui lòng truy cập hệ thống để kiểm tra chi tiết.</p>
    ";

            var subjectToSender = "Xác nhận cập nhật lịch thành công";
            var courseInfoUpdateSender = dto.CourseId.HasValue ? $"<p><strong>Khóa học:</strong> [Khóa học ID: {dto.CourseId}]</p>" : "";
            var bodyToSender = $@"
        <h3>Cập nhật lịch học thành công!</h3>
        <p>Bạn đã cập nhật lịch học với <strong>{receiver.Username} ({receiver.Email})</strong>.</p>
        <p><strong>Thời gian mới:</strong> {timeStr}</p>
        {courseInfoUpdateSender}
        <p>Thông báo đã được gửi đến người kia.</p>
    ";

            try
            {
                await _emailService.SendEmailAsync(receiver.Email, subjectToReceiver, bodyToReceiver);
                await _emailService.SendEmailAsync(user.Email, subjectToSender, bodyToSender);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email khi cập nhật lịch: {ex.Message}");
            }

            return Ok(new { message = "Cập nhật lịch thành công và đã gửi email thông báo." });
        }

    }
}
