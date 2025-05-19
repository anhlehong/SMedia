using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace SMedia.WebSocketHandlers;

public class WebSocketConnectionManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _connections = new();

    public void AddConnection(string user_id, WebSocket webSocket)
    {
        _connections.TryAdd(user_id, webSocket);
    }

    public void RemoveConnection(string user_id)
    {
        _connections.TryRemove(user_id, out _);
    }

    public WebSocket GetConnection(string user_id)
    {
        _connections.TryGetValue(user_id, out var webSocket);
        return webSocket;
    }

    public IEnumerable<string> GetAllConnectedUserIds()
    {
        return _connections.Keys;
    }
}