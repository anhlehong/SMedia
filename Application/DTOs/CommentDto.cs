namespace Application.DTOs;

public class CommentDto
{
    public Guid CommentId { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime? PostedAt { get; set; }
    public List<CommentDto> ChildComments { get; set; } = new List<CommentDto>();
}

