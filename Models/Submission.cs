using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class Submission
{
    public int SubmissionId { get; set; }

    public int AssignmentId { get; set; }

    public int StudentId { get; set; }

    public int AttemptNumber { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public string? SubmissionContent { get; set; }

    public decimal? Grade { get; set; }

    public string? Feedback { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual User Student { get; set; } = null!;

    public virtual ICollection<SubmissionFile> SubmissionFiles { get; set; } = new List<SubmissionFile>();
}
