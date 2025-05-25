using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace SMedia.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        Log.Information("Gửi tin nhắn từ {User}: {Message}", user, message);
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public override async Task OnConnectedAsync()
    {
        Log.Information("Client kết nối: {ConnectionId}, User: {UserId}", Context.ConnectionId, Context.User?.Identity?.Name ?? "Ẩn danh");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Log.Error(exception, "Client ngắt kết nối: {ConnectionId}, Lỗi: {ErrorMessage}", Context.ConnectionId, exception?.Message);
        await base.OnDisconnectedAsync(exception);
    }
}