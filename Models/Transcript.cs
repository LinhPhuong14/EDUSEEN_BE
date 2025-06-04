using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class Transcript
{
    public int TranscriptId { get; set; }

    public int CallId { get; set; }

    public string? Content { get; set; }

    public string? RecordingUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual VideoCall Call { get; set; } = null!;
}
