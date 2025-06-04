using System;
using System.Collections.Generic;

namespace Sep490_Eduseen_BE.Models;

public partial class ChatMessage
{
    public int MessageId { get; set; }

    public int CourseId { get; set; }

    public int SenderId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime? SentAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
