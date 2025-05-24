namespace Application.DTOs;

public class PostDtos
{
    public class MediaDto
    {
        public Guid MediaId { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public DateTime? UploadedAt { get; set; }
        public Guid UploadedBy { get; set; }
    }

    public class MediaCreateDto
    {
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
    }

    public class PostCreateDto
    {
        public string Content { get; set; } = string.Empty;
        public MediaCreateDto[] Media { get; set; } = Array.Empty<MediaCreateDto>();
    }

    public class GroupPostCreateDto : PostCreateDto
    {
        public Guid GroupId { get; set; }
    }

    public class PostUserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
    }

    public class PendingPostMediaDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }    public class PendingPostDto
    {
        public Guid Id { get; set; }
        public PostUserDto User { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public List<PendingPostMediaDto> Media { get; set; } = new List<PendingPostMediaDto>();
        public DateTime CreatedAt { get; set; }
        public Guid GroupId { get; set; }
        public bool IsVisible { get; set; }
    }

    public class PostDto
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public Guid? GroupId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsVisible { get; set; }
        public MediaDto[] Media { get; set; } = Array.Empty<MediaDto>();
        public int VoteCount { get; set; }
        public bool IsVotedByCurrentUser { get; set; }
        public int CommentCount { get; set; }
    }
    
    public class PostImageDto
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string userName { get; set; } = string.Empty; 
        public string? userAvatar { get; set; } 
        public string Content { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public Guid? GroupId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsVisible { get; set; }
        public MediaDto[] Media { get; set; } = Array.Empty<MediaDto>();
        public int VoteCount { get; set; }
        public bool IsVotedByCurrentUser { get; set; }
        public int CommentCount { get; set; }
    }
    

    public class PostUpdateDto
    {
        public string Content { get; set; } = string.Empty;
        public MediaCreateDto[] Media { get; set; } = Array.Empty<MediaCreateDto>();
    }

    public class StaticCommentCreateDto
    {
        public string Content { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }
    }

    public class StaticCommentDto
    {
        public Guid CommentId { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
    }    
    
    public class GroupPostApproveDto
    {
        public Guid PostId { get; set; }
        public bool Approve { get; set; } // true: Approved, false: Rejected
    }

    public class GroupPostVisibilityDto
    {
        public Guid PostId { get; set; }
        public bool IsVisible { get; set; }
    }
}