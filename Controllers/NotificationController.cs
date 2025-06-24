using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Dtos.Notification;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly Sep490EduseenContext _context;

        public NotificationController(Sep490EduseenContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetNotificationsForUser(int userId)
        {
            // Lấy danh sách thông báo
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            // Đánh dấu là đã đọc (nếu chưa đọc)
            foreach (var notification in notifications.Where(n => n.IsRead == false))
            {
                notification.IsRead = true;
            }

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            // Chuyển đổi sang DTO
            var dtoList = notifications.Select(n => new NotificationDTO
            {
                NotificationId = n.NotificationId,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            }).ToList();

            return Ok(dtoList);
        }

    }
}
