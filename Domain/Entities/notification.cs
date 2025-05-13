using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class notification
{
    public string notification_id { get; set; } = null!;

    public string user_id { get; set; } = null!;

    public string type { get; set; } = null!;

    public string related_user_id { get; set; } = null!;

    public string? related_post_id { get; set; }

    public string? related_message_id { get; set; }

    public DateTime? posted_at { get; set; }

    public bool? is_read { get; set; }

    public virtual message? related_message { get; set; }

    public virtual post? related_post { get; set; }

    public virtual user related_user { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}
