using Bungalov.Business.Interfaces;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;

namespace Bungalov.Business.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Rivora Rezervasyon", "karaliavla43@gmail.com"));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                // Gmail SMTP Ayarları
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("karaliavla43@gmail.com", "pwlz engx hhec llnu");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            _logger.LogInformation($"E-Posta gönderildi: {toEmail} - Konu: {subject}");
        }
        catch (Exception ex)
        {
            // E-posta hatası uygulamayı durdurmamalı, sadece loglanmalı
            _logger.LogError($"E-Posta gönderilemedi: {ex.Message}");
        }
    }
}
