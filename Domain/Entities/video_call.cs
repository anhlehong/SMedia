using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class video_call
{
    public string call_id { get; set; } = null!;

    public string caller_id { get; set; } = null!;

    public string? receiver_id { get; set; }

    public string? group_chat_id { get; set; }

    public DateTime? start_time { get; set; }

    public DateTime? end_time { get; set; }

    public string? status { get; set; }

    public virtual user caller { get; set; } = null!;

    public virtual group_chat? group_chat { get; set; }

    public virtual user? receiver { get; set; }
}
