namespace Application.DTOs;

public class FollowDto
{
    public Guid FollowerId { get; set; }
    public Guid FollowedId { get; set; }
    public DateTime? FollowedAt { get; set; }
    public UserDto Follower { get; set; } = null!;
    public UserDto Followed { get; set; } = null!;
}