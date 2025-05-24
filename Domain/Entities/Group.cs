using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Group
{
    public Guid GroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public string Visibility { get; set; } = null!;

    public Guid CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Image { get; set; }

    public virtual User Creator { get; set; } = null!;

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
