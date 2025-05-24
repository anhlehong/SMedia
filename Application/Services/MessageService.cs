// using Application.DTOs;
// using Application.Interfaces.RepositoryInterfaces;
// using Application.Interfaces.ServiceInterfaces;
// using Domain.Entities;
//
//
//
// namespace Application.Services;
//
// public class MessageService 
// {
//     private readonly IMessageRepository _messageRepository;
//     private readonly IUserRepository _userRepository;
//
//     public MessageService(IMessageRepository messageRepository, IUserRepository userRepository)
//     {
//         _messageRepository = messageRepository;
//         _userRepository = userRepository;
//     }
//
//     public async Task<MessageResponseDto> SendMessage(Guid senderId, SendMessageDto messageDto)
//     {
//         // Verify the sender exists
//         var sender = await _userRepository.GetUserById(senderId);
//         if (sender == null)
//         {
//             return new MessageResponseDto
//             {
//                 Success = false,
//                 Message = "Sender not found"
//             };
//         }
//
//         // Verify receiver if specified
//         User? receiver = null;
//         if (!string.IsNullOrEmpty(messageDto.ReceiverId))
//         {
//             receiver = await _userRepository.GetUserById(messageDto.ReceiverId);
//             if (receiver == null)
//             {
//                 return new MessageResponseDto
//                 {
//                     Success = false,
//                     Message = "Receiver not found"
//                 };
//             }
//         }
//
//         // Create the message
//         var message = new Message
//         {
//             MessageId = Guid.NewGuid(),
//             SenderId = senderId,
//             ReceiverId = messageDto.ReceiverId,
//             GroupChatId = messageDto.GroupChatId,
//             Content = messageDto.Content,
//             Media = messageDto.MediaType,
//             media_url = messageDto.MediaUrl,
//             sent_time = DateTime.UtcNow,
//             is_read = false
//         };
//
//         await _messageRepository.CreateMessage(message);
//
//         return new MessageResponseDto
//         {
//             Success = true,
//             Message = "Message sent successfully",
//             Data = new MessageDto
//             {
//                 MessageId = message.message_id,
//                 SenderId = message.sender_id,
//                 SenderName = sender.username,
//                 SenderImage = sender.image,
//                 ReceiverId = message.receiver_id,
//                 ReceiverName = receiver?.username,
//                 ReceiverImage = receiver?.image,
//                 GroupChatId = message.group_chat_id,
//                 Content = message.content,
//                 MediaType = message.media_type,
//                 MediaUrl = message.media_url,
//                 SentTime = message.sent_time,
//                 IsRead = message.is_read
//             }
//         };
//     }
//
//     public async Task<GetMessagesResponseDto> GetMessagesBetweenUsers(string currentUserId, string otherUserId, int page = 1, int pageSize = 20)
//     {
//         int skip = (page - 1) * pageSize;
//         var messages = await _messageRepository.GetMessagesBetweenUsers(currentUserId, otherUserId, skip, pageSize);
//         var totalCount = await _messageRepository.GetMessageCountBetweenUsers(currentUserId, otherUserId);
//
//         var messageDtos = messages.Select(m => new MessageDto
//         {
//             MessageId = m.message_id,
//             SenderId = m.sender_id,
//             SenderName = m.sender.username,
//             SenderImage = m.sender.image,
//             ReceiverId = m.receiver_id,
//             ReceiverName = m.receiver?.username,
//             ReceiverImage = m.receiver?.image,
//             GroupChatId = m.group_chat_id,
//             Content = m.content,
//             MediaType = m.media_type,
//             MediaUrl = m.media_url,
//             SentTime = m.sent_time,
//             IsRead = m.is_read
//         }).ToList();
//
//         return new GetMessagesResponseDto
//         {
//             Messages = messageDtos,
//             TotalCount = totalCount
//         };
//     }
//
//     public async Task<bool> MarkMessagesAsRead(string senderId, string receiverId)
//     {
//         return await _messageRepository.MarkMessagesAsRead(senderId, receiverId);
//     }
//
//     public async Task<List<MessageDto>> GetUnreadMessages(string userId)
//     {
//         var messages = await _messageRepository.GetUnreadMessages(userId);
//
//         return messages.Select(m => new MessageDto
//         {
//             MessageId = m.message_id,
//             SenderId = m.sender_id,
//             SenderName = m.sender.username,
//             SenderImage = m.sender.image,
//             ReceiverId = m.receiver_id,
//             Content = m.content,
//             MediaType = m.media_type,
//             MediaUrl = m.media_url,
//             SentTime = m.sent_time,
//             IsRead = m.is_read
//         }).ToList();
//     }
//
//     public async Task<List<user>> GetChatUsers(string userId, int page = 1, int pageSize = 20)
//     {
//         int skip = (page - 1) * pageSize;
//         return await _messageRepository.GetChatUsers(userId, skip, pageSize);
//     }
// }

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

    public async Task<MessageDto> SaveAndBroadcastMessageAsync(MessageDto messageDto)
    {
        // Ánh xạ MessageDto thành entity message
        var message = messageDto.Adapt<message>();

        // Lưu tin nhắn vào database
        await _messageRepository.SaveMessageAsync(message);

        // Trả về DTO đã lưu
        return message.Adapt<MessageDto>();
    }

    public async Task<List<string>> GetGroupMemberIdsAsync(string group_chat_id)
    {
        if (string.IsNullOrEmpty(group_chat_id))
        {
            throw new ArgumentNullException(nameof(group_chat_id), "Group chat ID cannot be null or empty.");
        }

        return await _messageRepository.GetGroupMemberIdsAsync(group_chat_id);
    }
    
    public async Task<List<UserSearchDto>> SearchUsersAsync(string searchTerm, string currentUserId)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return new List<UserSearchDto>();
        }

        var blockedUsers = await _messageRepository.GetBlockedUserIdsAsync(currentUserId);

        var users = await _messageRepository.SearchUsersAsync(searchTerm, currentUserId, blockedUsers);
        return users.Adapt<List<UserSearchDto>>();
    }
}

