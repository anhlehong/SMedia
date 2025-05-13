using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Application.Interfaces.ServiceInterfaces;
using Application.DTOs;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;

namespace Application.Services;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfig;
    private readonly IMemoryCache _memoryCache;

    public EmailService(EmailConfiguration emailConfig, IMemoryCache memoryCache)
    {
        _emailConfig = emailConfig ?? throw new ArgumentNullException(nameof(emailConfig));
        _memoryCache = memoryCache;
    }

    // Send OTP email to user
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        using (var smtpClient = new SmtpClient())
        {
            await smtpClient.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_emailConfig.SenderEmail, _emailConfig.Password);

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(_emailConfig.SenderName, _emailConfig.SenderEmail));
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart("plain") { Text = message };

            mailMessage.To.Add(new MailboxAddress("", toEmail));

            await smtpClient.SendAsync(mailMessage);
            await smtpClient.DisconnectAsync(true);
        }
    }

    // Generate OTP and store it in MemoryCache with TTL of 2 minutes
    public async Task<string> GenerateOtpAsync(string email)
    {
        var otp = GenerateRandomOtp();
        var otpData = new { OTP = otp, Attempts = 0, CreatedAt = DateTime.UtcNow };

        var key = $"otp_{email}";
        _memoryCache.Set(key, otpData, TimeSpan.FromMinutes(2));

        return otp;
    }

    public async Task<bool> VerifyOtpAsync(string email, string otp)
    {
        var key = $"otp_{email}";

        if (!_memoryCache.TryGetValue(key, out dynamic otpData))
            return false;

        var storedOtp = otpData.OTP;
        var attempts = otpData.Attempts;
        var createdAt = otpData.CreatedAt;

        if (attempts >= 5)
        {
            _memoryCache.Remove(key);
            return false;
        }

        if (DateTime.UtcNow > createdAt.AddMinutes(2))
        {
            _memoryCache.Remove(key);
            return false;
        }

        if (storedOtp != otp)
        {
            otpData.Attempts += 1;

            _memoryCache.Set(key, (object)otpData, TimeSpan.FromMinutes(2));

            return false;
        }

        _memoryCache.Remove(key);
        return true;
    }

    private string GenerateRandomOtp()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            var otp = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % 1000000;
            return otp.ToString("D6");
        }
    }
}