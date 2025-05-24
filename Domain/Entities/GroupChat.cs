using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class GroupChat
{
    public Guid GroupChatId { get; set; }

    public string ChatName { get; set; } = null!;

    public Guid CreatedBy { get; set; }

    public DateTime? StartedAt { get; set; }

    public string? ImageUrl  { get; set; }

    public virtual User Creator { get; set; } = null!;

    public virtual ICollection<GroupChatMember> GroupChatMembers { get; set; } = new List<GroupChatMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
