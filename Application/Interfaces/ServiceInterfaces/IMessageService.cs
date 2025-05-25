using Application.DTOs;

namespace Application.Interfaces.ServiceInterfaces;

public interface IMessageService
{
    Task<MessageDto> SendMessageAsync(Guid senderId, MessageDto messageDto);
    Task<List<MessageDto>> GetUnreadMessagesAsync(Guid receiverId);
    Task<List<MessageDto>> GetMessageHistoryAsync(Guid userId, Guid? receiverId, Guid? groupChatId, int page, int pageSize);
    Task MarkMessageAsReadAsync(Guid messageId);
}