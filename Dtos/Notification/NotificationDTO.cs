namespace Sep490_Eduseen_BE.Dtos.Notification
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public bool? IsRead { get; set; }
    }

    public class MarkAsReadRequestDTO
    {
        public int NotificationId { get; set; }
    }

    public class MarkMultipleAsReadRequestDTO
    {
        public List<int> NotificationIds { get; set; } = new List<int>();
    }

    public class DeleteNotificationRequestDTO
    {
        public int NotificationId { get; set; }
    }

    public class DeleteMultipleNotificationsRequestDTO
    {
        public List<int> NotificationIds { get; set; } = new List<int>();
    }

    public class NotificationCountDTO
    {
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
    }
}
