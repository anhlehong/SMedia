namespace Application.DTOs;

public class UserUpdateDto
{
    public string? FullName { get; set; }
    public string? Intro { get; set; }
    public string? Gender { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? Image { get; set; }
}