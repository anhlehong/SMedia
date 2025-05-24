using Application.DTOs;

namespace Application.Interfaces.ServiceInterfaces;

public interface IUserService
{
    Task<bool> IsEmailExists(string email);
    Task<bool> IsUsernameExists(string username);
    Task<Alert?> Register(RegisterDto registerDto);
    Task<AuthResponse?> Login(LoginDto loginDto);
    // --------------------------------------------------------------------------------------------------------------------
    Task<UserDto> UpdateUserAsync(Guid userId, UserUpdateDto updateDto);
    Task<UserDto> GetUserByIdAsync(Guid userId);
}
