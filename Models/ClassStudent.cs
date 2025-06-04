using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class ClassStudent
{
    public int ClassStudentId { get; set; }

    public int ClassId { get; set; }

    public int StudentId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
