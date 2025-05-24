using Application.DTOs;
using DotNetEnv;

namespace SMedia.Configuration;

public static class EmailConfigurationExtensions
{
    public static IServiceCollection AddEmailConfiguration(this IServiceCollection services)
    {
        // Cấu hình email từ .env
        var emailConfig = new EmailConfiguration
        {
            SmtpServer = Env.GetString("EMAIL_SMTP_SERVER") ?? throw new InvalidOperationException("EMAIL_SMTP_SERVER chưa được thiết lập trong .env"),
            Port = int.TryParse(Env.GetString("EMAIL_PORT"), out var port) ? port : throw new InvalidOperationException("EMAIL_PORT chưa được thiết lập hoặc không hợp lệ trong .env"),
            SenderEmail = Env.GetString("EMAIL_SENDER_EMAIL") ?? throw new InvalidOperationException("EMAIL_SENDER_EMAIL chưa được thiết lập trong .env"),
            Password = Env.GetString("EMAIL_PASSWORD") ?? throw new InvalidOperationException("EMAIL_PASSWORD chưa được thiết lập trong .env"),
            SenderName = Env.GetString("EMAIL_SENDER_NAME") ?? throw new InvalidOperationException("EMAIL_SENDER_NAME chưa được thiết lập trong .env")
        };

        // Đăng ký EmailConfiguration như singleton
        services.AddSingleton(emailConfig);

        return services;
    }
}