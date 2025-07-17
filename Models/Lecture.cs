using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class Lecture
{
    public int LectureId { get; set; }

    public int SectionId { get; set; }

    public string Title { get; set; } = null!;

    public string? ContentType { get; set; }

    public string? ContentUrl { get; set; }

    public int? Duration { get; set; }

    public int Order { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Section Section { get; set; } = null!;

    public virtual ICollection<UserLectureProgress> UserLectureProgresses { get; set; } = new List<UserLectureProgress>();
}
