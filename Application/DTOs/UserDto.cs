namespace Application.DTOs;

public class UserDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Intro { get; set; }
    public string? Image { get; set; }
    public DateTime? JoinedAt { get; set; }
}