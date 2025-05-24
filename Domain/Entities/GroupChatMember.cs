using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class GroupChatMember
{
    public Guid GroupChatId { get; set; }

    public Guid UserId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public string? Role { get; set; }

    public string? Status { get; set; }

    public virtual GroupChat GroupChat { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
