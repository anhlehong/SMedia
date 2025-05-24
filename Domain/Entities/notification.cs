using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Notification
{
    public Guid NotificationId { get; set; }

    public Guid UserId { get; set; }

    public string Type { get; set; } = null!;

    public Guid RelatedUserId { get; set; }

    public Guid? RelatedPostId { get; set; }

    public Guid? RelatedMessageId { get; set; }

    public DateTime? NotifiedAt { get; set; }

    public bool? IsRead { get; set; }

    public virtual Message? RelatedMessage { get; set; }

    public virtual Post? RelatedPost { get; set; }

    public virtual User RelatedUser { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
