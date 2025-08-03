public class ScheduleRequestDTO
{
    public string ReceiverEmail { get; set; } = string.Empty;
    public DateTime ScheduledTime { get; set; }
    public int Duration { get; set; }
    //public string Note { get; set; } = string.Empty;
    public int? CourseId { get; set; } 
}

