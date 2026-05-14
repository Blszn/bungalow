using System;

namespace Bungalov.Core.Varliklar;

public class ChatMessage : BaseEntity
{
    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsAdminMessage { get; set; }
    public bool IsEdited { get; set; }
    public DateTime SentDate { get; set; } = DateTime.Now;
}
