using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SMedia.Configuration;

public static class JwtConfiguration
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        // Đọc cấu hình JWT từ .env
        var jwtConfig = new Application.DTOs.JwtConfiguration()
        {
            Issuer = Env.GetString("JWT_ISSUER") ?? throw new InvalidOperationException("JWT_ISSUER chưa được thiết lập trong .env"),
            Audience = Env.GetString("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT_AUDIENCE chưa được thiết lập trong .env"),
            Key = Env.GetString("JWT_KEY") ?? throw new InvalidOperationException("JWT_KEY chưa được thiết lập trong .env")
        };

        // Đăng ký JwtConfiguration như singleton
        services.AddSingleton(jwtConfig);

        // Cấu hình xác thực JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
                };
            });

        return services;
    }
}
