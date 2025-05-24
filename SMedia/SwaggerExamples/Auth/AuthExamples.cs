using Application.DTOs;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Any;

namespace SMedia.SwaggerExamples.Auth;

public class LoginDtoExample : IMultipleExamplesProvider<LoginDto>
{
    public IEnumerable<SwaggerExample<LoginDto>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Login Credentials",
            "Example of valid login credentials",
            new LoginDto
            {
                Email = "user@example.com",
                Password = "Password123!"
            });
    }
}

public class RegisterDtoExample : IMultipleExamplesProvider<RegisterDto>
{
    public IEnumerable<SwaggerExample<RegisterDto>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "User Registration",
            "Example of complete user registration information",
            new RegisterDto
            {
                Username = "johndoe",
                Email = "john.doe@example.com",
                Password = "StrongPassword123!",
                FullName = "John Doe",
                Gender = "Male",
                Birthday = new DateOnly(1990, 1, 1)
            });
    }
}

public class AuthResponseExample : IMultipleExamplesProvider<AuthResponse>
{
    public IEnumerable<SwaggerExample<AuthResponse>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "JWT Authentication Token",
            "Example of successful authentication response containing a JWT token",
            new AuthResponse
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
            });
    }
}

public class AlertExample : IMultipleExamplesProvider<Alert>
{
    public IEnumerable<SwaggerExample<Alert>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Success Alert",
            "Example of a success message alert",
            new Alert
            {
                Message = "Operation completed successfully",
                AlertType = "success"
            });
            
        yield return SwaggerExample.Create(
            "Error Alert",
            "Example of an error message alert",
            new Alert
            {
                Message = "Operation failed",
                AlertType = "error"
            });
            
        yield return SwaggerExample.Create(
            "Info Alert",
            "Example of an informational message alert",
            new Alert
            {
                Message = "Please check your email to confirm your account",
                AlertType = "info"
            });
    }
}
