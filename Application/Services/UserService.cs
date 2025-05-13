using Application.DTOs;
using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Domain.Entities;
using Mapster;
using System.Text.Json;
using MapsterMapper;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public UserService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }
    
    public async Task<bool> IsEmailExists(string email)
    {
        return await _userRepository.IsEmailExists(email);
    }
    
    public async Task<bool> IsUsernameExists(string username)
    {
        return await _userRepository.IsUsernameExists(username);
    }

    public async Task<Alert?> Register(RegisterDto registerDto)
    {
        var newUser = registerDto.Adapt<user>();
        await _userRepository.Create(newUser);

        return new Alert()
        {
            Message = "Registration successful.",
            AlertType = "success"
        };
    }
    public async Task<AuthResponse?> Login(LoginDto loginDto)
    {
        var user = await _userRepository.GetUserByEmail(loginDto.Email);
        if (user == null || !user.VerifyPassword(loginDto.Password))
            return null; 

        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponse()
        {
            Token = token
        };
    }
}