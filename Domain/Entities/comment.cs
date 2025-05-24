using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Comment
{
    public Guid CommentId { get; set; }

    public Guid PostId { get; set; }

    public Guid UserId { get; set; }

    public Guid? ParentCommentId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? PostedAt { get; set; }

    public virtual ICollection<Comment> ChildComments { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
