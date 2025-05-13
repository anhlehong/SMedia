using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class message
{
    public string message_id { get; set; } = null!;

    public string sender_id { get; set; } = null!;

    public string? receiver_id { get; set; }

    public string? group_chat_id { get; set; }

    public string? content { get; set; }

    public string? media_type { get; set; }

    public string? media_url { get; set; }

    public DateTime? sent_time { get; set; }

    public bool? is_read { get; set; }

    public virtual group_chat? group_chat { get; set; }

    public virtual ICollection<notification> notifications { get; set; } = new List<notification>();

    public virtual user? receiver { get; set; }

    public virtual user sender { get; set; } = null!;
}
