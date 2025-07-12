using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Dtos.VideoCall;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Services
{
    public class VideoCallService : IVideoCallService
    {
        private readonly Sep490EduseenContext _context;

        public VideoCallService(Sep490EduseenContext context)
        {
            _context = context;
        }

        public async Task<List<VideoCallHistoryDTO>> GetVideoCallHistoryForUser(int userId)
        {
            var calls = await _context.VideoCalls
                .Include(vc => vc.Schedule)
                    .ThenInclude(s => s.Course)
                .Include(vc => vc.Schedule.Student)
                .Include(vc => vc.Schedule.Teacher)
                .Where(vc => vc.Schedule.StudentId == userId || vc.Schedule.TeacherId == userId)
                .OrderByDescending(vc => vc.StartTime)
                .Select(vc => new VideoCallHistoryDTO
                {
                    CallId = vc.CallId,
                    Topic = $"Cuộc gọi với Giáo viên {vc.Schedule.Teacher.FirstName} {vc.Schedule.Teacher.LastName}",
                    Subject = vc.Schedule.Course.Title,
                    StartTime = vc.StartTime ?? DateTime.MinValue,
                    EndTime = vc.EndTime ?? DateTime.MinValue,
                    DurationMinutes = vc.StartTime.HasValue && vc.EndTime.HasValue
                        ? (vc.EndTime.Value - vc.StartTime.Value).TotalMinutes
                        : 0,
                    TeacherName = $"{vc.Schedule.Teacher.FirstName} {vc.Schedule.Teacher.LastName}",
                    StudentName = $"{vc.Schedule.Student.FirstName} {vc.Schedule.Student.LastName}",
                    Note = vc.Schedule.Status 
                })
                .ToListAsync();

            return calls;
        }
    }
}
