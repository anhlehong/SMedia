using Domain.Entities;

namespace Application.Interfaces.RepositoryInterfaces;

public interface IMessageRepository
{
    Task<Message> AddMessageAsync(Message message);
    Task<List<Message>> GetUnreadMessagesAsync(Guid receiverId);
    Task<List<Message>> GetMessagesByUserAsync(Guid userId, Guid? receiverId, Guid? groupChatId, int skip, int take);
    Task UpdateMessageAsync(Message message);
    // fix: Thêm phương thức GetMessageByIdAsync để lấy Message theo ID
    Task<Message> GetMessageByIdAsync(Guid messageId);
}