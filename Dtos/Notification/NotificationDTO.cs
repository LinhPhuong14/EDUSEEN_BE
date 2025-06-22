namespace Sep490_Eduseen_BE.Dtos.Notification
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public bool? IsRead { get; set; }
    }
}
