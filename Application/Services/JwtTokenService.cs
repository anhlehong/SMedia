using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtConfiguration _jwtConfig;

    public JwtTokenService(JwtConfiguration jwtConfig)
    {
        _jwtConfig = jwtConfig;
    }

    public string GenerateToken(user user)
    {
        var claims = new[]
        {
            new Claim("user_id", user.user_id),
            new Claim("image", user.image ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}