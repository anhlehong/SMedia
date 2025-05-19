using Application.DTOs;

namespace Application.Interfaces.ServiceInterfaces;

public interface IMessageService
{
    Task<MessageDto> SaveAndBroadcastMessageAsync(MessageDto messageDto);
    Task<List<string>> GetGroupMemberIdsAsync(string group_chat_id);
    Task<List<UserSearchDto>> SearchUsersAsync(string searchTerm, string currentUserId);
}