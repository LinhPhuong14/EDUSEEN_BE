using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class VideoCall
{
    public int CallId { get; set; }

    public int ScheduleId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Status { get; set; }

    public virtual Schedule Schedule { get; set; } = null!;

    public virtual ICollection<Transcript> Transcripts { get; set; } = new List<Transcript>();
}
