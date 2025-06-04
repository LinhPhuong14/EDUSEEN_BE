using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int CourseId { get; set; }

    public int StudentId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<ReviewResponse> ReviewResponses { get; set; } = new List<ReviewResponse>();

    public virtual User Student { get; set; } = null!;
}
