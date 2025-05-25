using System.Net.WebSockets;
using System.Text;
using Serilog;

namespace SMedia.Realtime;

public class WebSocketHandler
{
    private readonly WebSocketConnectionManager _connectionManager;

    public WebSocketHandler(WebSocketConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task HandleAsync(string userId, WebSocket webSocket)
    {
        _connectionManager.AddConnection(userId, webSocket);
        Log.Information("User {UserId} connected", userId);

        // Thông báo người dùng tham gia
        await BroadcastMessage($"[{DateTime.Now:HH:mm:ss}] User {userId} joined the chat");

        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            try
            {   
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var broadcastMessage = $"[{DateTime.Now:HH:mm:ss}] {userId}: {message}";
                    Log.Information("Received from {UserId}: {Message}", userId, message);
                    await BroadcastMessage(broadcastMessage);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Log.Information("User {UserId} disconnected", userId);
                    _connectionManager.RemoveConnection(userId);
                    await BroadcastMessage($"[{DateTime.Now:HH:mm:ss}] User {userId} left the chat");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                    break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error handling WebSocket for {UserId}", userId);
                break;
            }
        }
    }

    private async Task BroadcastMessage(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        foreach (var socket in _connectionManager.GetAllConnections())
        {
            if (socket.Value.State == WebSocketState.Open)
            {
                try
                {
                    await socket.Value.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error broadcasting to {UserId}", socket.Key);
                }
            }
        }
    }
}