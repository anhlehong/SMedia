using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class User
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string? DeletedUserEmail { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public DateTime? JoinedAt { get; set; }

    public string? Status { get; set; }

    public string? Intro { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Gender { get; set; }

    public string? Image { get; set; }
    
    public void SetPassword(string password)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<GroupChatMember> GroupChatMembers { get; set; } = new List<GroupChatMember>();

    public virtual ICollection<GroupChat> GroupChats { get; set; } = new List<GroupChat>();

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Media> UploadedMedia { get; set; } = new List<Media>();

    public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();

    public virtual ICollection<Notification> CreatedNotifications { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();

    public virtual ICollection<PostVote> PostVotes { get; set; } = new List<PostVote>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<UserFollow> Followings { get; set; } = new List<UserFollow>();

    public virtual ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
}
