// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using SMedia.WebSocketHandlers;
//
// namespace SMedia.Configuration;
//
// public static class WebSocketConfiguration
// {
//     public static IServiceCollection AddWebSocketServices(this IServiceCollection services)
//     {
//         services.AddSingleton<WebSocketConnectionManager>();
//         services.AddSingleton<SimpleChatWebSocketHandler>();
//         return services;
//     }
//
//     public static IApplicationBuilder UseWebSocketHandler(this IApplicationBuilder app)
//     {
//         app.Use(async (context, next) =>
//         {
//             if (context.Request.Path == "/ws")
//             {
//                 if (context.WebSockets.IsWebSocketRequest)
//                 {
//                     var userId = context.Request.Query["userId"];
//                     if (string.IsNullOrEmpty(userId))
//                     {
//                         context.Response.StatusCode = 400;
//                         await context.Response.WriteAsync("Missing userId");
//                         return;
//                     }
//
//                     var webSocket = await context.WebSockets.AcceptWebSocketAsync();
//                     var connectionManager = context.RequestServices.GetRequiredService<WebSocketConnectionManager>();
//                     var handler = context.RequestServices.GetRequiredService<SimpleChatWebSocketHandler>();
//
//                     await handler.HandleAsync(userId, webSocket);
//                 }
//                 else
//                 {
//                     context.Response.StatusCode = 400;
//                     await context.Response.WriteAsync("Invalid WebSocket request");
//                 }
//             }
//             else
//             {
//                 await next();
//             }
//         });
//
//         return app;
//     }
// }