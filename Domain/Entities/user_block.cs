using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class user_block
{
    public string blocker_id { get; set; } = null!;

    public string blocked_id { get; set; } = null!;

    public DateTime? blocked_time { get; set; }

    public virtual user blocked { get; set; } = null!;

    public virtual user blocker { get; set; } = null!;
}
