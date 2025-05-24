using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PostVote
{
    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public string? VoteType { get; set; }

    public DateTime? VotedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
