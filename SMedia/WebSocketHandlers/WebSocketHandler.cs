using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace SMedia.WebSocketHandlers;

public class WebSocketHandler
{
    private readonly WebSocketConnectionManager _connectionManager;
    private readonly IServiceProvider _serviceProvider;

    public WebSocketHandler(WebSocketConnectionManager connectionManager, IServiceProvider serviceProvider)
    {
        _connectionManager = connectionManager;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(string user_id, WebSocket webSocket)
    {
        _connectionManager.AddConnection(user_id, webSocket);

        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await ProcessMessageAsync(user_id, messageJson);

            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        _connectionManager.RemoveConnection(user_id);
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    private async Task ProcessMessageAsync(string sender_id, string messageJson)
    {
        try
        {
            // Deserialize JSON thành MessageDto
            var messageDto = JsonSerializer.Deserialize<MessageDto>(messageJson);
            if (messageDto == null)
            {
                return; // Bỏ qua nếu không deserialize được
            }

            // Gán sender_id từ WebSocket
            messageDto.sender_id = sender_id;

            // Lưu và gửi tin nhắn
            using var scope = _serviceProvider.CreateScope();
            var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();

            var savedMessage = await messageService.SaveAndBroadcastMessageAsync(messageDto);

            // Kiểm tra và gửi tin nhắn đến người nhận hoặc nhóm
            if (!string.IsNullOrEmpty(messageDto.receiver_id))
            {
                await SendToUserAsync(messageDto.receiver_id, savedMessage);
            }
            else if (!string.IsNullOrEmpty(messageDto.group_chat_id))
            {
                await SendToGroupAsync(messageDto.group_chat_id, savedMessage, sender_id);
            }
        }
        catch (Exception ex)
        {
            // Log lỗi nếu cần
            Console.WriteLine($"Error processing message: {ex.Message}");
        }
    }

    private async Task SendToUserAsync(string receiver_id, MessageDto message)
    {
        var ws = _connectionManager.GetConnection(receiver_id);
        if (ws != null && ws.State == WebSocketState.Open)
        {
            var messageJson = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(messageJson);
            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    private async Task SendToGroupAsync(string group_chat_id, MessageDto message, string sender_id)
    {
        using var scope = _serviceProvider.CreateScope();
        var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
        var memberIds = await messageService.GetGroupMemberIdsAsync(group_chat_id);

        foreach (var memberId in memberIds)
        {
            if (memberId != sender_id) // Không gửi lại cho người gửi
            {
                await SendToUserAsync(memberId, message);
            }
        }
    }
}