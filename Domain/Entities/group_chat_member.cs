using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class group_chat_member
{
    public string group_chat_id { get; set; } = null!;

    public string user_id { get; set; } = null!;

    public DateTime? joined_time { get; set; }

    public virtual group_chat group_chat { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}
