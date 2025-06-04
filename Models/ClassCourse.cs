using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class ClassCourse
{
    public int ClassCourseId { get; set; }

    public int ClassId { get; set; }

    public int CourseId { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
