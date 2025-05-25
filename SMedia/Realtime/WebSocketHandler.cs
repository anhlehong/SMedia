using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;
using Serilog;

namespace SMedia.Realtime;

public class WebSocketHandler
{
    private readonly WebSocketConnectionManager _connectionManager;

    // fix: Thêm IServiceProvider để tạo scope thủ công
    private readonly IServiceProvider _serviceProvider;

    // fix: Sửa constructor để nhận IServiceProvider thay vì IMessageService và INotificationService trực tiếp
    public WebSocketHandler(WebSocketConnectionManager connectionManager, IServiceProvider serviceProvider)
    {
        _connectionManager = connectionManager;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(string userId, WebSocket webSocket)
    {
        _connectionManager.AddConnection(userId, webSocket);
        Log.Information("User {UserId} connected", userId);

        await BroadcastMessage($"[{DateTime.Now:HH:mm:ss}] User {userId} joined the chat");

        // fix: Tạo scope để lấy IMessageService và INotificationService
        using (var scope = _serviceProvider.CreateScope())
        {
            var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            // Gửi các tin nhắn chưa đọc
            var unreadMessages = await messageService.GetUnreadMessagesAsync(Guid.Parse(userId));
            int maxUnreadMessages = Math.Min(unreadMessages.Count, 50);
            for (int i = 0; i < maxUnreadMessages; i++)
            {
                var message = unreadMessages[i];
                var formattedMessage = $"[{message.SentAt:HH:mm:ss}] {message.SenderId}: {message.Content}";
                await SendMessage(userId, formattedMessage);
                await messageService.MarkMessageAsReadAsync(message.MessageId);
            }

            // Gửi các thông báo chưa đọc
            var unreadNotifications = await notificationService.GetUnreadNotificationsAsync(Guid.Parse(userId));
            int maxUnreadNotifications = Math.Min(unreadNotifications.Count, 50);
            for (int i = 0; i < maxUnreadNotifications; i++)
            {
                var notification = unreadNotifications[i];
                var formattedNotification =
                    $"[{notification.NotifiedAt:HH:mm:ss}] Notification: {notification.Type} from {notification.RelatedUserId}";
                await SendMessage(userId, formattedNotification);
                await notificationService.MarkNotificationAsReadAsync(notification.NotificationId);
            }
        }

        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var request = JsonSerializer.Deserialize<ClientRequest>(messageJson, options);

                    if (request == null || string.IsNullOrEmpty(request.Action))
                    {
                        Log.Warning("Invalid request format from {UserId}: {Message}", userId, messageJson);
                        continue;
                    }

                    // fix: Tạo scope để lấy IMessageService và INotificationService cho mỗi request từ client
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        switch (request.Action.ToLower())
                        {
                            case "send":
                                var messageObj = JsonSerializer.Deserialize<MessageRequest>(messageJson, options);
                                if (messageObj == null || string.IsNullOrEmpty(messageObj.ReceiverId) ||
                                    string.IsNullOrEmpty(messageObj.Content))
                                {
                                    Log.Warning("Failed to deserialize message from {UserId}: {Message}", userId,
                                        messageJson);
                                    continue;
                                }

                                var messageDto = new MessageDto
                                {
                                    MessageId = Guid.NewGuid(),
                                    ReceiverId = Guid.Parse(messageObj.ReceiverId),
                                    Content = messageObj.Content,
                                    IsRead = false,
                                    IsVisible = true
                                };

                                var savedMessage =
                                    await messageService.SendMessageAsync(Guid.Parse(userId), messageDto);

                                Log.Information("Received from {SenderId} to {ReceiverId}: {Message}", userId,
                                    messageObj.ReceiverId, messageObj.Content);

                                var formattedMessage =
                                    $"[{savedMessage.SentAt:HH:mm:ss}] {userId}: {savedMessage.Content}";

                                var receiverSocket = _connectionManager.GetConnectionByUserId(messageObj.ReceiverId);
                                if (receiverSocket != null && receiverSocket.State == WebSocketState.Open)
                                {
                                    await SendMessage(messageObj.ReceiverId, formattedMessage);
                                    await messageService.MarkMessageAsReadAsync(savedMessage.MessageId);
                                }
                                else
                                {
                                    var existingNotifications =
                                        await notificationService.GetUnreadNotificationsAsync(
                                            Guid.Parse(messageObj.ReceiverId));
                                    if (!existingNotifications.Any(n => n.RelatedMessageId == savedMessage.MessageId))
                                    {
                                        await notificationService.CreateNotificationAsync(
                                            Guid.Parse(messageObj.ReceiverId),
                                            Guid.Parse(userId),
                                            savedMessage.MessageId,
                                            "NewMessage"
                                        );
                                    }
                                }

                                await SendMessage(userId, formattedMessage);
                                break;

                            case "gethistory":
                                var historyRequest = JsonSerializer.Deserialize<HistoryRequest>(messageJson, options);
                                if (historyRequest == null || (string.IsNullOrEmpty(historyRequest.ReceiverId) &&
                                                               string.IsNullOrEmpty(historyRequest.GroupChatId)))
                                {
                                    Log.Warning("Invalid history request from {UserId}: {Message}", userId,
                                        messageJson);
                                    continue;
                                }

                                Guid? receiverId = string.IsNullOrEmpty(historyRequest.ReceiverId)
                                    ? null
                                    : Guid.Parse(historyRequest.ReceiverId);
                                Guid? groupChatId = string.IsNullOrEmpty(historyRequest.GroupChatId)
                                    ? null
                                    : Guid.Parse(historyRequest.GroupChatId);
                                int page = historyRequest.Page > 0 ? historyRequest.Page : 1;
                                int pageSize = historyRequest.PageSize > 0 ? historyRequest.PageSize : 20;

                                var messages = await messageService.GetMessageHistoryAsync(Guid.Parse(userId),
                                    receiverId, groupChatId, page, pageSize);
                                foreach (var message in messages)
                                {
                                    var historyMessage =
                                        $"[{message.SentAt:HH:mm:ss}] {message.SenderId}: {message.Content}";
                                    await SendMessage(userId, historyMessage);
                                    if (!message.IsRead.GetValueOrDefault() && message.ReceiverId == Guid.Parse(userId))
                                    {
                                        await messageService.MarkMessageAsReadAsync(message.MessageId);
                                    }
                                }

                                break;

                            default:
                                Log.Warning("Unknown action from {UserId}: {Action}", userId, request.Action);
                                break;
                        }
                    }
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

    private async Task SendMessage(string userId, string message)
    {
        var socket = _connectionManager.GetConnectionByUserId(userId);
        if (socket != null && socket.State == WebSocketState.Open)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
            Log.Information("Sent message to {UserId}: {Message}", userId, message);
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

    public class MessageRequest
    {
        public string Action { get; set; } = "send";
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }

    public class HistoryRequest
    {
        public string Action { get; set; } = "gethistory";
        public string ReceiverId { get; set; }
        public string GroupChatId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class ClientRequest
    {
        public string Action { get; set; }
    }
}