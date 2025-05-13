using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class comment
{
    public string comment_id { get; set; } = null!;

    public string post_id { get; set; } = null!;

    public string user_id { get; set; } = null!;

    public string? parent_comment_id { get; set; }

    public string content { get; set; } = null!;

    public DateTime? posted_at { get; set; }

    public virtual ICollection<comment> Inverseparent_comment { get; set; } = new List<comment>();

    public virtual comment? parent_comment { get; set; }

    public virtual post post { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}
