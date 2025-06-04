using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class Favorite
{
    public int FavoriteId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
