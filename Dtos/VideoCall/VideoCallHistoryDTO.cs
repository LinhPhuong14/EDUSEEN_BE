namespace Sep490_Eduseen_BE.Dtos.VideoCall
{
    public class VideoCallHistoryDTO
    {
        public int CallId { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double DurationMinutes { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
