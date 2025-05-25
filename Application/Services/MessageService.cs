using Application.DTOs;
using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Domain.Entities;
using Mapster;

namespace Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<MessageDto> SendMessageAsync(Guid senderId, MessageDto messageDto)
    {
        var message = messageDto.Adapt<Message>();
        message.SenderId = senderId;
        message.SentAt = DateTime.UtcNow;
        message.IsRead = false;

        var savedMessage = await _messageRepository.AddMessageAsync(message);
        return savedMessage.Adapt<MessageDto>();
    }

    public async Task<List<MessageDto>> GetUnreadMessagesAsync(Guid receiverId)
    {
        var messages = await _messageRepository.GetUnreadMessagesAsync(receiverId);
        return messages.Adapt<List<MessageDto>>();
    }

    public async Task<List<MessageDto>> GetMessageHistoryAsync(Guid userId, Guid? receiverId, Guid? groupChatId, int page, int pageSize)
    {
        int skip = (page - 1) * pageSize;
        var messages = await _messageRepository.GetMessagesByUserAsync(userId, receiverId, groupChatId, skip, pageSize);
        return messages.Adapt<List<MessageDto>>();
    }

    public async Task MarkMessageAsReadAsync(Guid messageId)
    {
        var message = await _messageRepository.GetMessageByIdAsync(messageId);
        if (message != null)
        {
            message.IsRead = true;
            await _messageRepository.UpdateMessageAsync(message);
        }
    }
}