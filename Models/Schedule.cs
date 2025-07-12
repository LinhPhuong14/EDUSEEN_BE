using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int StudentId { get; set; }

    public int TeacherId { get; set; }

    public DateTime ScheduledTime { get; set; }

    public int Duration { get; set; }

    public string Status { get; set; } = null!;

    public virtual User Student { get; set; } = null!;

    public virtual User Teacher { get; set; } = null!;

    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<VideoCall> VideoCalls { get; set; } = new List<VideoCall>();
}
