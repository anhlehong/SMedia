using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class user_follow
{
    public string follower_id { get; set; } = null!;

    public string followed_id { get; set; } = null!;

    public DateTime? followed_time { get; set; }

    public virtual user followed { get; set; } = null!;

    public virtual user follower { get; set; } = null!;
}
