using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class post
{
    public string post_id { get; set; } = null!;

    public string user_id { get; set; } = null!;

    public string content { get; set; } = null!;

    public string? status { get; set; }

    public DateTime? posted_at { get; set; }

    public virtual ICollection<comment> comments { get; set; } = new List<comment>();

    public virtual ICollection<notification> notifications { get; set; } = new List<notification>();

    public virtual ICollection<post_vote> post_votes { get; set; } = new List<post_vote>();

    public virtual user user { get; set; } = null!;
}
