using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class Section
{
    public int SectionId { get; set; }

    public int CourseId { get; set; }

    public string Title { get; set; } = null!;

    public int Order { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
}
