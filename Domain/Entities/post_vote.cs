using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class post_vote
{
    public string user_id { get; set; } = null!;

    public string post_id { get; set; } = null!;

    public string? vote_type { get; set; }

    public DateTime? voted_time { get; set; }

    public virtual post post { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}
