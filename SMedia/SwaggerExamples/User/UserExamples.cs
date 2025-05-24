// using Domain.Entities;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace SMedia.SwaggerExamples.User;

public class UserSearchResultExample : IMultipleExamplesProvider<List<object>>
{
    public IEnumerable<SwaggerExample<List<object>>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "User search results",
            "Example of user search results",
            new List<object>
            {
                new
                {
                    userId = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
                    username = "johndoe",
                    fullName = "John Doe",
                    email = "john.doe@example.com",                image = "https://example.com/profile-images/john.jpg"
                },
                new
                {
                    userId = "a47bc10b-58cc-4372-a567-0e02b2c3d123",
                    username = "janedoe",
                    fullName = "Jane Doe",
                    email = "jane.doe@example.com",
                    image = "https://example.com/profile-images/jane.jpg"
                }
            });
    }
}

public class UserProfileExample : IMultipleExamplesProvider<object>
{
    public IEnumerable<SwaggerExample<object>> GetExamples()
    {
        var example = new
        {
            userId = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
            username = "johndoe",
            fullName = "John Doe",
            email = "john.doe@example.com",
            image = "https://example.com/profile-images/john.jpg",
            intro = "Hello, I'm John and I'm a software developer.",
            birthday = new DateOnly(1990, 1, 15),
            gender = "Male",
            joinedAt = DateTime.UtcNow.AddYears(-2)
        };
        
        yield return SwaggerExample.Create<object>(
            "User Profile",
            "Complete information about a user profile",
            example);
    }
}
