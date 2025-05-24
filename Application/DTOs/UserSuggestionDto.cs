namespace Application.DTOs;

public class UserSuggestionDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Image { get; set; }
    public int MutualFollowersCount { get; set; }
}