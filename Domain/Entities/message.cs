using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Message
{
    public Guid MessageId { get; set; }

    public Guid SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    public Guid? GroupChatId { get; set; }

    public string? Content { get; set; }

    public DateTime? SentAt  { get; set; }

    public bool? IsRead { get; set; }

    public bool? IsVisible { get; set; }

    public virtual GroupChat? GroupChat { get; set; }

    public virtual ICollection<Media> Media { get; set; } = new List<Media>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User? Receiver { get; set; }

    public virtual User Sender { get; set; } = null!;
}
