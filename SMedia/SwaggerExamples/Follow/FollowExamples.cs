// using Application.DTOs;
// using Swashbuckle.AspNetCore.Filters;
// using System;
// using System.Collections.Generic;
// using Microsoft.OpenApi.Any;
//
// namespace SMedia.SwaggerExamples.Follow;
//
// public class FollowDtoExample : IMultipleExamplesProvider<FollowDto>
// {
//     public IEnumerable<SwaggerExample<FollowDto>> GetExamples()
//     {
//         yield return SwaggerExample.Create(
//             "Follow Information",
//             "Example of a follow relationship",
//             new FollowDto
//             {
//                 UserId = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
//                 Username = "janedoe",
//                 FullName = "Jane Doe",
//                 Image = "https://example.com/profile-images/jane.jpg",
//                 FollowedTime = DateTime.UtcNow.AddDays(-3)
//             });
//     }
// }
//
// public class FollowRequestDtoExample : IMultipleExamplesProvider<FollowRequestDto>
// {
//     public IEnumerable<SwaggerExample<FollowRequestDto>> GetExamples()
//     {
//         yield return SwaggerExample.Create(
//             "Follow Request", 
//             "Request to follow a user",
//             new FollowRequestDto
//             {
//                 UserId = "f47ac10b-58cc-4372-a567-0e02b2c3d479"
//             });
//     }
// }
//
// public class FollowResponseDtoExample : IMultipleExamplesProvider<FollowResponseDto>
// {
//     public IEnumerable<SwaggerExample<FollowResponseDto>> GetExamples()
//     {
//         yield return SwaggerExample.Create(
//             "Successful Follow Response",
//             "Example of a successful follow operation response",
//             new FollowResponseDto
//             {
//                 Success = true,
//                 Message = "User followed successfully",
//                 Follow = new FollowDto
//                 {
//                     UserId = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
//                     Username = "janedoe",
//                     FullName = "Jane Doe",
//                     Image = "https://example.com/profile-images/jane.jpg",
//                     FollowedTime = DateTime.UtcNow
//                 }
//             });
//             
//         yield return SwaggerExample.Create(
//             "Failed Follow Response",
//             "Example of a failed follow operation response",
//             new FollowResponseDto
//             {
//                 Success = false,
//                 Message = "User not found or you cannot follow this user",
//                 Follow = null
//             });
//     }
// }
//
// public class GetFollowersResponseDtoExample : IMultipleExamplesProvider<GetFollowersResponseDto>
// {
//     public IEnumerable<SwaggerExample<GetFollowersResponseDto>> GetExamples()
//     {
//         yield return SwaggerExample.Create(
//             "User Followers List",
//             "Example response showing a user's followers",
//             new GetFollowersResponseDto
//             {
//                 Followers = new List<FollowDto>
//                 {
//                     new FollowDto
//                     {                        UserId = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
//                         Username = "janedoe",
//                         FullName = "Jane Doe",
//                         Image = "https://example.com/profile-images/jane.jpg",
//                         FollowedTime = DateTime.UtcNow.AddDays(-3)
//                     },
//                     new FollowDto
//                     {
//                         UserId = "c47ac10b-58cc-4372-a567-0e02b2c3d123",
//                         Username = "robertsmith",
//                         FullName = "Robert Smith",
//                         Image = "https://example.com/profile-images/robert.jpg",
//                         FollowedTime = DateTime.UtcNow.AddDays(-5)
//                     }
//                 },
//                 TotalCount = 2
//             });
//     }
// }
//
// public class GetFollowingResponseDtoExample : IMultipleExamplesProvider<GetFollowingResponseDto>
// {
//     public IEnumerable<SwaggerExample<GetFollowingResponseDto>> GetExamples()
//     {
//         yield return SwaggerExample.Create(
//             "User Following List",
//             "Example response showing users that a user is following",
//             new GetFollowingResponseDto
//             {
//                 Following = new List<FollowDto>
//                 {
//                     new FollowDto
//                     {
//                         UserId = "d47ac10b-58cc-4372-a567-0e02b2c3d789",
//                         Username = "mikejohnson",
//                         FullName = "Mike Johnson",
//                         Image = "https://example.com/profile-images/mike.jpg",
//                         FollowedTime = DateTime.UtcNow.AddDays(-2)                    },
//                     new FollowDto
//                     {
//                         UserId = "e47ac10b-58cc-4372-a567-0e02b2c3d456",
//                         Username = "sarahwilson",
//                         FullName = "Sarah Wilson",
//                         Image = "https://example.com/profile-images/sarah.jpg",
//                         FollowedTime = DateTime.UtcNow.AddDays(-7)
//                     }
//                 },
//                 TotalCount = 2
//             });
//     }
// }
