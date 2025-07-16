using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class Assignment
{
    public int AssignmentId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int LectureId { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Lecture Lecture { get; set; } = null!;

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
