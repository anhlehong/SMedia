using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Post
{
    public Guid PostId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? PostedAt { get; set; }

    public Guid? GroupId { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsVisible { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Group? Group { get; set; }

    public virtual ICollection<Media> Media { get; set; } = new List<Media>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PostVote> PostVotes { get; set; } = new List<PostVote>();

    public virtual User User { get; set; } = null!;
}
