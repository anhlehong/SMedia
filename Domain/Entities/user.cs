using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class user
{
    public string user_id { get; set; } = null!;

    public string username { get; set; } = null!;

    public string? email { get; set; }

    public string? deleted_user_email { get; set; }

    public string password_hash { get; set; } = null!;

    public string? full_name { get; set; }

    public DateTime? joined_at { get; set; }

    public string? status { get; set; }

    public string? intro { get; set; }

    public DateOnly? birthday { get; set; }

    public string? gender { get; set; }

    public string? image { get; set; }

    public virtual ICollection<comment> comments { get; set; } = new List<comment>();

    public virtual ICollection<group_chat_member> group_chat_members { get; set; } = new List<group_chat_member>();

    public virtual ICollection<group_chat> group_chats { get; set; } = new List<group_chat>();

    public virtual ICollection<message> messagereceivers { get; set; } = new List<message>();

    public virtual ICollection<message> messagesenders { get; set; } = new List<message>();

    public virtual ICollection<notification> notificationrelated_users { get; set; } = new List<notification>();

    public virtual ICollection<notification> notificationusers { get; set; } = new List<notification>();

    public virtual ICollection<post_vote> post_votes { get; set; } = new List<post_vote>();

    public virtual ICollection<post> posts { get; set; } = new List<post>();

    public virtual ICollection<user_block> user_blockblockeds { get; set; } = new List<user_block>();

    public virtual ICollection<user_block> user_blockblockers { get; set; } = new List<user_block>();

    public virtual ICollection<user_follow> user_followfolloweds { get; set; } = new List<user_follow>();

    public virtual ICollection<user_follow> user_followfollowers { get; set; } = new List<user_follow>();

    public virtual ICollection<video_call> video_callcallers { get; set; } = new List<video_call>();

    public virtual ICollection<video_call> video_callreceivers { get; set; } = new List<video_call>();
    
    public void SetPassword(string password)
    {
        password_hash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, password_hash);
    }
}
