using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class EmailConfirmationToken
{
    public int EmailConfirmId { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public virtual User User { get; set; } = null!;
}
