using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SMedia.Realtime;


namespace SMedia.Configuration;

public static class RealtimeConfiguration
{
    public static IServiceCollection AddRealtimeServices(this IServiceCollection services)
    {
        services.AddSingleton<WebSocketConnectionManager>();
        services.AddSingleton<WebSocketHandler>(sp =>
            new WebSocketHandler(sp.GetRequiredService<WebSocketConnectionManager>(), sp));

        return services;
    }

    public static IApplicationBuilder UseRealtimeHandler(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    // fix_token: Lấy token từ query parameter thay vì header Authorization
                    var token = context.Request.Query["token"].ToString();
                    if (string.IsNullOrEmpty(token))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Missing token in query parameter");
                        return;
                    }

                    // fix_token: Xác thực token thủ công thay vì dùng context.User
                    var userId = ValidateToken(token, context.RequestServices);
                    if (string.IsNullOrEmpty(userId))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid token");
                        return;
                    }

                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    var connectionManager = context.RequestServices.GetRequiredService<WebSocketConnectionManager>();
                    var handler = context.RequestServices.GetRequiredService<WebSocketHandler>();

                    await handler.HandleAsync(userId, webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid WebSocket request");
                }
            }
            else
            {
                await next();
            }
        });

        return app;
    }

    // fix_token: Thêm phương thức ValidateToken để xác thực token từ query parameter
    private static string ValidateToken(string token, IServiceProvider serviceProvider)
    {
        try
        {
            var jwtConfig = serviceProvider.GetRequiredService<Application.DTOs.JwtConfiguration>();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtConfig.Key);

            tokenHandler.ValidateToken(token, new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims.First(x => x.Type == "user_id").Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return null;
        }
    }
}