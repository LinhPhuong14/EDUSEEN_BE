using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Dtos.Notification;
using System.Security.Claims;

namespace Sep490_Eduseen_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly Sep490EduseenContext _context;

        public NotificationController(Sep490EduseenContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tạo dữ liệu test cho notifications (chỉ dùng cho development)
        /// </summary>
        [HttpPost("create-test-data")]
        public async Task<IActionResult> CreateTestData()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out var userId))
                {
                    return Unauthorized("User ID claim is missing or invalid.");
                }

                var testNotifications = new List<Notification>
                {
                    new Notification
                    {
                        UserId = userId,
                        Message = "Chào mừng bạn đến với EDUSEEN! Hãy khám phá các khóa học mới.",
                        CreatedAt = DateTime.Now.AddHours(-2),
                        IsRead = false
                    },
                    new Notification
                    {
                        UserId = userId,
                        Message = "Khóa học 'Lập trình React' đã được cập nhật với nội dung mới.",
                        CreatedAt = DateTime.Now.AddHours(-1),
                        IsRead = false
                    },
                    new Notification
                    {
                        UserId = userId,
                        Message = "Bạn có 1 bài tập mới trong khóa học 'JavaScript cơ bản'.",
                        CreatedAt = DateTime.Now.AddMinutes(-30),
                        IsRead = true
                    },
                    new Notification
                    {
                        UserId = userId,
                        Message = "Hẹn gặp lại bạn trong buổi học trực tuyến ngày mai!",
                        CreatedAt = DateTime.Now.AddMinutes(-15),
                        IsRead = false
                    },
                    new Notification
                    {
                        UserId = userId,
                        Message = "Chúc mừng! Bạn đã hoàn thành khóa học 'HTML & CSS'.",
                        CreatedAt = DateTime.Now.AddMinutes(-5),
                        IsRead = false
                    }
                };

                await _context.Notifications.AddRangeAsync(testNotifications);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Đã tạo {testNotifications.Count} thông báo test" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo dữ liệu test", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách thông báo của user (không tự động đánh dấu đã đọc)
        /// </summary>
        [HttpGet("user")]
        public async Task<IActionResult> GetNotificationsForUser([FromQuery] bool? isRead = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out var userId))
                {
                    return Unauthorized("User ID claim is missing or invalid.");
                }

                var query = _context.Notifications.Where(n => n.UserId == userId);

                // Lọc theo trạng thái đã đọc nếu có
                if (isRead.HasValue)
                {
                    query = query.Where(n => n.IsRead == isRead.Value);
                }

                // Sắp xếp theo thời gian tạo mới nhất
                query = query.OrderByDescending(n => n.CreatedAt);

                // Phân trang
                var totalCount = await query.CountAsync();
                var notifications = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Chuyển đổi sang DTO
                var dtoList = notifications.Select(n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                }).ToList();

                return Ok(new
                {
                    notifications = dtoList,
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách thông báo", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy số lượng thông báo của user
        /// </summary>
        [HttpGet("user/count")]
        public async Task<IActionResult> GetNotificationCount()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out var userId))
                {
                    return Unauthorized("User ID claim is missing or invalid.");
                }

                var totalCount = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .CountAsync();

                var unreadCount = await _context.Notifications
                    .Where(n => n.UserId == userId && n.IsRead == false)
                    .CountAsync();

                var result = new NotificationCountDTO
                {
                    TotalCount = totalCount,
                    UnreadCount = unreadCount
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy số lượng thông báo", error = ex.Message });
            }
        }

        /// <summary>
        /// Đánh dấu một thông báo đã đọc
        /// </summary>
        [HttpPut("mark-as-read")]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadRequestDTO request)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out var userId))
                {
                    return Unauthorized("User ID claim is missing or invalid.");
                }

                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationId == request.NotificationId && n.UserId == userId);

                if (notification == null)
                {
                    return NotFound(new { message = "Không tìm thấy thông báo" });
                }

                notification.IsRead = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã đánh dấu thông báo đã đọc" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi đánh dấu thông báo đã đọc", error = ex.Message });
            }
        }

        /// <summary>
        /// Đánh dấu nhiều thông báo đã đọc
        /// </summary>
        [HttpPut("mark-multiple-as-read")]
        public async Task<IActionResult> MarkMultipleAsRead([FromBody] MarkMultipleAsReadRequestDTO request)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out var userId))
                {
                    return Unauthorized("User ID claim is missing or invalid.");
                }

                if (request.NotificationIds == null || !request.NotificationIds.Any())
                {
                    return BadRequest(new { message = "Danh sách ID thông báo không được để trống" });
                }

                var notifications = await _context.Notifications
                    .Where(n => request.NotificationIds.Contains(n.NotificationId) && n.UserId == userId)
                    .ToListAsync();

                if (!notifications.Any())
                {
                    return NotFound(new { message = "Không tìm thấy thông báo nào" });
                }

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = $"Đã đánh dấu {notifications.Count} thông báo đã đọc" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi đánh dấu thông báo đã đọc", error = ex.Message });
            }
        }

        /// <summary>
        /// Đánh dấu tất cả thông báo của user đã đọc
        /// </summary>
        [HttpPut("user/mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out var userId))
                {
                    return Unauthorized("User ID claim is missing or invalid.");
                }

                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && n.IsRead == false)
                    .ToListAsync();

                if (!notifications.Any())
                {
                    return Ok(new { message = "Không có thông báo nào chưa đọc" });
                }

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = $"Đã đánh dấu {notifications.Count} thông báo đã đọc" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi đánh dấu tất cả thông báo đã đọc", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa một thông báo
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteNotification([FromBody] DeleteNotificationRequestDTO request)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out var userId))
                {
                    return Unauthorized("User ID claim is missing or invalid.");
                }

                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationId == request.NotificationId && n.UserId == userId);

                if (notification == null)
                {
                    return NotFound(new { message = "Không tìm thấy thông báo" });
                }

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã xóa thông báo thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa thông báo", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa nhiều thông báo
        /// </summary>
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleNotifications([FromBody] DeleteMultipleNotificationsRequestDTO request)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out var userId))
                {
                    return Unauthorized("User ID claim is missing or invalid.");
                }

                if (request.NotificationIds == null || !request.NotificationIds.Any())
                {
                    return BadRequest(new { message = "Danh sách ID thông báo không được để trống" });
                }

                var notifications = await _context.Notifications
                    .Where(n => request.NotificationIds.Contains(n.NotificationId) && n.UserId == userId)
                    .ToListAsync();

                if (!notifications.Any())
                {
                    return NotFound(new { message = "Không tìm thấy thông báo nào" });
                }

                _context.Notifications.RemoveRange(notifications);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Đã xóa {notifications.Count} thông báo thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa thông báo", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa tất cả thông báo của user
        /// </summary>
        [HttpDelete("user/delete-all")]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out var userId))
                {
                    return Unauthorized("User ID claim is missing or invalid.");
                }

                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .ToListAsync();

                if (!notifications.Any())
                {
                    return Ok(new { message = "Không có thông báo nào để xóa" });
                }

                _context.Notifications.RemoveRange(notifications);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Đã xóa {notifications.Count} thông báo thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa tất cả thông báo", error = ex.Message });
            }
        }
    }
}
