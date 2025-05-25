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
                    if (!context.User.Identity.IsAuthenticated)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }

                    var userId = context.User.FindFirst("user_id")?.Value;
                    if (string.IsNullOrEmpty(userId))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid userId in token");
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
}