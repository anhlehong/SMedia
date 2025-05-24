// using Application.DTOs;
// using Swashbuckle.AspNetCore.Filters;
// using System;
// using System.Collections.Generic;
// using Microsoft.OpenApi.Any;
//
// namespace SMedia.SwaggerExamples.Message;
//
// public class MessageDtoExample : IMultipleExamplesProvider<MessageDto>
// {
//     public IEnumerable<SwaggerExample<MessageDto>> GetExamples()
//     {
//         yield return SwaggerExample.Create(
//             "Standard Message",
//             "Example of a standard message between users",
//             new MessageDto
//             {
//                 MessageId = "e47ac10b-58cc-4372-a567-0e02b2c3d111",
//                 SenderId = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
//                 SenderName = "johndoe",
//                 SenderImage = "https://example.com/profile-images/john.jpg",
//                 ReceiverId = "d47ac10b-58cc-4372-a567-0e02b2c3d789",
//                 ReceiverName = "janedoe",
//                 ReceiverImage = "https://example.com/profile-images/jane.jpg",            Content = "Hello, how are you today?",
//                 SentTime = DateTime.UtcNow.AddMinutes(-15),
//                 IsRead = false
//             });
//     }
// }
//
// public class SendMessageDtoExample : IMultipleExamplesProvider<SendMessageDto>
// {
//     public IEnumerable<SwaggerExample<SendMessageDto>> GetExamples()
//     {
//         yield return SwaggerExample.Create(
//             "New Message", 
//             "Example of sending a new text message to another user",
//             new SendMessageDto
//             {
//                 ReceiverId = "d47ac10b-58cc-4372-a567-0e02b2c3d789",
//                 Content = "Hello, how are you today?",
//                 MediaType = "text"
//             });
//     }
// }
//
// public class MessageResponseDtoExample : IMultipleExamplesProvider<MessageResponseDto>
// {
//     public IEnumerable<SwaggerExample<MessageResponseDto>> GetExamples()
//     {
//         yield return SwaggerExample.Create(
//             "Successful Message Response",
//             "Example of a successful message sending response",
//             new MessageResponseDto
//             {
//                 Success = true,
//                 Message = "Message sent successfully",
//                 Data = new MessageDto
//                 {
//                     MessageId = "e47ac10b-58cc-4372-a567-0e02b2c3d111",
//                     SenderId = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
//                     SenderName = "johndoe",
//                     SenderImage = "https://example.com/profile-images/john.jpg",
//                     ReceiverId = "d47ac10b-58cc-4372-a567-0e02b2c3d789",
//                     ReceiverName = "janedoe",
//                     ReceiverImage = "https://example.com/profile-images/jane.jpg",
//                     Content = "Hello, how are you today?",
//                     SentTime = DateTime.UtcNow,
//                     IsRead = false
//                 }
//             });
//
//         yield return SwaggerExample.Create(
//             "Failed Message Response",
//             "Example of a failed message sending response",
//             new MessageResponseDto
//             {
//                 Success = false,
//                 Message = "Failed to send message. Receiver not found.",
//                 Data = null
//             });
//     }
// }
//
// public class GetMessagesResponseDtoExample : IMultipleExamplesProvider<GetMessagesResponseDto>
// {
//     public IEnumerable<SwaggerExample<GetMessagesResponseDto>> GetExamples()
//     {
//         yield return SwaggerExample.Create(
//             "Message Conversation History",
//             "Example of conversation history between two users",
//             new GetMessagesResponseDto
//             {
//                 Messages = new List<MessageDto>
//                 {
//                     new MessageDto                    {
//                         MessageId = "e47ac10b-58cc-4372-a567-0e02b2c3d111",
//                         SenderId = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
//                         SenderName = "johndoe",
//                         SenderImage = "https://example.com/profile-images/john.jpg",
//                         ReceiverId = "d47ac10b-58cc-4372-a567-0e02b2c3d789",
//                         ReceiverName = "janedoe",
//                         ReceiverImage = "https://example.com/profile-images/jane.jpg",
//                         Content = "Hello, how are you today?",
//                         SentTime = DateTime.UtcNow.AddMinutes(-15),
//                         IsRead = true
//                     },
//                     new MessageDto
//                     {
//                         MessageId = "e47ac10b-58cc-4372-a567-0e02b2c3d222",
//                         SenderId = "d47ac10b-58cc-4372-a567-0e02b2c3d789",
//                         SenderName = "janedoe",
//                         SenderImage = "https://example.com/profile-images/jane.jpg",
//                         ReceiverId = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
//                         ReceiverName = "johndoe",
//                         ReceiverImage = "https://example.com/profile-images/john.jpg",
//                         Content = "I'm good, thanks for asking! How about you?",
//                         SentTime = DateTime.UtcNow.AddMinutes(-14),
//                         IsRead = false
//                     }                },
//                 TotalCount = 2
//             });
//     }
// }
