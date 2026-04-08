using Bungalov.Business.Interfaces;
using Microsoft.Extensions.Logging;

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
        // Gerçek SMTP ayarları buraya eklenebilir. 
        // Şimdilik simülasyon olarak log kaydı alıyoruz.
        _logger.LogInformation($"E-Posta Gönderildi: {toEmail} - Konu: {subject}");
        await Task.CompletedTask;
    }
}
