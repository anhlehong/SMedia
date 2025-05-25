using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace SMedia.Realtime;

public class WebSocketConnectionManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _connections = new();

    public void AddConnection(string userId, WebSocket webSocket)
    {
        _connections.TryAdd(userId, webSocket);
    }

    public void RemoveConnection(string userId)
    {
        _connections.TryRemove(userId, out _);
    }

    public ConcurrentDictionary<string, WebSocket> GetAllConnections()
    {
        return _connections;
    }
    
    // fix1: Thêm phương thức để lấy WebSocket theo userId, dùng để gửi tin nhắn đến người nhận cụ thể
    public WebSocket GetConnectionByUserId(string userId)
    {
        _connections.TryGetValue(userId, out var webSocket);
        return webSocket;
    }
}