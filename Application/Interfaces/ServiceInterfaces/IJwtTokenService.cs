using Domain.Entities;

namespace Application.Interfaces.ServiceInterfaces;

public interface IJwtTokenService
{
    string GenerateToken(user user);
}