using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class SubmissionFile
{
    public int FileId { get; set; }

    public int SubmissionId { get; set; }

    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;

    public virtual Submission Submission { get; set; } = null!;
}
