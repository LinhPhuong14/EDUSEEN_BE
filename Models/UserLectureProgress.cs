using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class UserLectureProgress
{
    public int ProgressId { get; set; }

    public int UserId { get; set; }

    public int LectureId { get; set; }

    public bool? IsCompleted { get; set; }

    public DateTime? LastAccessed { get; set; }

    public virtual Lecture Lecture { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
