// using Application.DTOs;
// using Domain.Entities;
//
//
// namespace Application.Interfaces.ServiceInterfaces;
//
// public interface IMessageService
// {
//     Task<MessageResponseDto> SendMessage(string senderId, SendMessageDto messageDto);
//     Task<GetMessagesResponseDto> GetMessagesBetweenUsers(string currentUserId, string otherUserId, int page = 1, int pageSize = 20);
//     Task<bool> MarkMessagesAsRead(string senderId, string receiverId);
//     Task<List<MessageDto>> GetUnreadMessages(string userId);
//     Task<List<user>> GetChatUsers(string userId, int page = 1, int pageSize = 20);
// }
//

using Application.DTOs;

namespace Application.Interfaces.ServiceInterfaces;

public interface IMessageService
{
    Task<MessageDto> SaveAndBroadcastMessageAsync(MessageDto messageDto);
    Task<List<string>> GetGroupMemberIdsAsync(string group_chat_id);
    Task<List<UserSearchDto>> SearchUsersAsync(string searchTerm, string currentUserId);
}

