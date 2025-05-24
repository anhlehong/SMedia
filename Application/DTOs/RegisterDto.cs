namespace Application.DTOs;

public class RegisterDto
{
    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Gender { get; set; }
}