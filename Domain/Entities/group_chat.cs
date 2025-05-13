using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class group_chat
{
    public string group_chat_id { get; set; } = null!;

    public string group_name { get; set; } = null!;

    public string created_by { get; set; } = null!;

    public DateTime? started_at { get; set; }

    public virtual user created_byNavigation { get; set; } = null!;

    public virtual ICollection<group_chat_member> group_chat_members { get; set; } = new List<group_chat_member>();

    public virtual ICollection<message> messages { get; set; } = new List<message>();

    public virtual ICollection<video_call> video_calls { get; set; } = new List<video_call>();
}
