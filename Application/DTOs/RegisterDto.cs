namespace Application.DTOs;

public class RegisterDto
{
    public string username { get; set; } = null!;

    public string? email { get; set; }

    public string password { get; set; } = null!;

    public string? full_name { get; set; }

    public DateOnly? birthday { get; set; }

    public string? gender { get; set; }
}