using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Media
{
    public Guid MediaId { get; set; }

    public string MediaUrl { get; set; } = null!;

    public string MediaType { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public Guid UploadedBy { get; set; }

    public Guid? PostId { get; set; }

    public Guid? MessageId { get; set; }

    public virtual Message? Message { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User UploadedByNavigation { get; set; } = null!;
}
