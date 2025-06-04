using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public string ClassCode { get; set; } = null!;

    public int TeacherId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ClassCourse> ClassCourses { get; set; } = new List<ClassCourse>();

    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

    public virtual User Teacher { get; set; } = null!;
}
