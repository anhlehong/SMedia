using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class UserFollow
{
    public Guid FollowerId { get; set; }

    public Guid FollowedId { get; set; }

    public DateTime? FollowedAt { get; set; }

    public virtual User Followed { get; set; } = null!;

    public virtual User Follower { get; set; } = null!;
}
