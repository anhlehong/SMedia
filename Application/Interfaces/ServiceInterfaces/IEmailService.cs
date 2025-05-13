namespace Application.Interfaces.ServiceInterfaces;

public interface IEmailService
{
    Task<string> GenerateOtpAsync(string email);

    Task SendEmailAsync(string toEmail, string subject, string message);

    Task<bool> VerifyOtpAsync(string email, string otp);
}