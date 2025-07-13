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
                CourseId = dto.CourseId
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Gửi email cho người được mời (receiver)
            var subjectToReceiver = "Bạn có một lịch gọi video mới!";
            var bodyToReceiver = $@"
                <h3>Lịch học mới từ EDUSEEN</h3>
                <p><strong>Thời gian:</strong> {dto.ScheduledTime:HH:mm dd/MM/yyyy}</p>
                <p><strong>Người đặt lịch:</strong> {sender.Username}({senderEmail})</p>
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

            //  Gửi email xác nhận đến người đặt lịch (sender)
            var subjectToSender = "Xác nhận đặt lịch gọi thành công";
            var bodyToSender = $@"
                <h3>Đặt lịch thành công!</h3>
                <p>Bạn đã đặt một lịch học với người dùng <strong>{receiver.Username}({receiver.Email})</strong>.</p>
                <p><strong>Thời gian:</strong> {dto.ScheduledTime:HH:mm dd/MM/yyyy}</p>
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
    }
}
