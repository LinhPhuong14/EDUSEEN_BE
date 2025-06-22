using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class Otp
{
    public int OtpId { get; set; }

    public int? UserId { get; set; }

    public string Email { get; set; } = null!;

    public string OtpCode { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsUsed { get; set; }

    public virtual User? User { get; set; }
}
