namespace Application.DTOs
{
    public class GroupCreateDto
    {
        public string GroupName { get; set; } = string.Empty;
        public string Visibility { get; set; } = "Public"; // Public or Private
        public string? Image { get; set; }
    }

    public class GroupDto
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Image { get; set; }
        public int MemberCount { get; set; }
    }

    public class GroupUpdateDto
    {
        public string GroupName { get; set; } = string.Empty;
        public string? Image { get; set; }
    }

    public class GroupMemberRequestDto
    {
        public Guid GroupId { get; set; }
    }

    public class GroupMemberDto
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime? JoinedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class GroupMemberApproveDto
    {
        public Guid UserId { get; set; }
        public bool Approve { get; set; } // true: Active, false: Rejected
    }
}