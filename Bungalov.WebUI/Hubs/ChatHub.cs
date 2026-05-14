using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Bungalov.Core.Varliklar;
using Bungalov.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Bungalov.WebUI.Hubs;

public class ChatHub : Hub
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private static readonly ConcurrentDictionary<string, string> _userConnections = new();

    public ChatHub(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    // ... (rest of the methods)
    
    public async Task SendMessageToSupport(string user, string message)
    {
        var displayName = user;
        string? userId = null;
        string email = user; // Varsayılan olarak gelen isim/email

        if (Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated)
        {
            var appUser = await _userManager.GetUserAsync(Context.User);
            if (appUser != null)
            {
                displayName = $"{appUser.FirstName} {appUser.LastName}";
                userId = appUser.Id;
                email = appUser.Email ?? user;
            }
        }

        // Bağlantıyı kaydet/güncelle
        _userConnections[email] = Context.ConnectionId;

        // Mesajı veritabanına kaydet
        var chatMsg = new ChatMessage
        {
            AppUserId = userId,
            SenderEmail = email,
            SenderName = displayName,
            Message = message,
            IsAdminMessage = false,
            SentDate = DateTime.Now
        };
        await _unitOfWork.GetRepository<ChatMessage>().AddAsync(chatMsg);
        await _unitOfWork.SaveAsync();

        Serilog.Log.Information("SendMessageToSupport called by {User}: {Message}", displayName, message);
        
        await Clients.Group("Support").SendAsync("ReceiveMessage", displayName, message, Context.ConnectionId, email, chatMsg.Id);
        
        // Gönderen kullanıcıya da ID'yi bildir (silme işlemi için lazım)
        await Clients.Caller.SendAsync("MessageSentConfirmation", chatMsg.Id);
    }

    public async Task SendResponseToUser(string connectionId, string user, string message, string targetEmail)
    {
        // Admin mesajını kaydet
        var chatMsg = new ChatMessage
        {
            SenderEmail = targetEmail, // Bu mesaj bu kullanıcıyla olan sohbete ait
            SenderName = user,
            Message = message,
            IsAdminMessage = true,
            SentDate = DateTime.Now
        };
        await _unitOfWork.GetRepository<ChatMessage>().AddAsync(chatMsg);
        await _unitOfWork.SaveAsync();

        Serilog.Log.Information("SendResponseToUser called for {ConnectionId}: {Message}", connectionId, message);
        
        // Eğer connectionId boşsa veya geçersizse, en güncel ID'yi sözlükten bulmaya çalışalım
        var finalConnId = connectionId;
        if (string.IsNullOrEmpty(finalConnId) || finalConnId == "null")
        {
            _userConnections.TryGetValue(targetEmail, out finalConnId);
        }

        if (!string.IsNullOrEmpty(finalConnId))
        {
            await Clients.Client(finalConnId).SendAsync("ReceiveResponse", user, message, chatMsg.Id);
        }
        
        // Admine de kendi mesajının ID'sini bildir (UI'da silme için lazım olabilir)
        await Clients.Caller.SendAsync("MessageSentConfirmation", chatMsg.Id);
    }

    public async Task<List<object>> GetChatHistory(string email)
    {
        var messages = await _unitOfWork.GetRepository<ChatMessage>()
            .GetByFilterAsync(m => m.SenderEmail == email);
            
        return messages
            .OrderBy(m => m.SentDate)
            .Select(m => new {
                m.Id,
                m.SenderName,
                m.Message,
                m.IsAdminMessage,
                m.IsEdited,
                m.SentDate
            })
            .Cast<object>()
            .ToList();
    }

    // Yazıyor bildirimi
    public async Task NotifyTyping(string targetEmail, bool isTyping)
    {
        if (string.IsNullOrEmpty(targetEmail)) return;

        var senderEmail = Context.User?.Identity?.Name ?? "Misafir";
        
        // Admine gönder
        await Clients.Group("Support").SendAsync("UserTyping", senderEmail, isTyping);
        
        // Kullanıcıya gönder
        if (_userConnections.TryGetValue(targetEmail, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("UserTyping", senderEmail, isTyping);
        }
    }

    // Mesajı tekil silme
    public async Task DeleteSingleMessage(int messageId)
    {
        var repo = _unitOfWork.GetRepository<ChatMessage>();
        var msg = await repo.GetByIdAsync(messageId);
        
        if (msg != null)
        {
            var isAdmin = Context.User != null && Context.User.IsInRole("Admin");
            
            // Kullanıcı kontrolü: Email üzerinden veya Identity üzerinden
            var currentUser = Context.User?.Identity?.Name ?? "Misafir";
            var isOwner = msg.SenderEmail == currentUser;
            
            if (isAdmin || isOwner)
            {
                var targetEmail = msg.SenderEmail;
                repo.Delete(msg);
                await _unitOfWork.SaveAsync();

                // Her iki tarafa da silindi bilgisini gönder
                await Clients.Group("Support").SendAsync("MessageDeleted", messageId);
                
                if (_userConnections.TryGetValue(targetEmail, out var connectionId))
                {
                    await Clients.Client(connectionId).SendAsync("MessageDeleted", messageId);
                }
            }
        }
    }

    // Mesajı düzenleme
    public async Task EditMessage(int messageId, string newMessage)
    {
        var repo = _unitOfWork.GetRepository<ChatMessage>();
        var msg = await repo.GetByIdAsync(messageId);
        
        if (msg != null && !string.IsNullOrWhiteSpace(newMessage))
        {
            var isAdmin = Context.User != null && Context.User.IsInRole("Admin");
            var currentUser = Context.User?.Identity?.Name ?? "Misafir";
            var isOwner = msg.SenderEmail == currentUser;
            
            if (isAdmin || isOwner)
            {
                msg.Message = newMessage;
                msg.IsEdited = true;
                repo.Update(msg);
                await _unitOfWork.SaveAsync();

                // Her iki tarafa da düzenlendi bilgisini gönder
                await Clients.Group("Support").SendAsync("MessageEdited", messageId, newMessage);
                
                if (_userConnections.TryGetValue(msg.SenderEmail, out var connectionId))
                {
                    await Clients.Client(connectionId).SendAsync("MessageEdited", messageId, newMessage);
                }
            }
        }
    }

    public async Task<List<object>> GetRecentChats()
    {
        var recentMessages = await _unitOfWork.GetRepository<ChatMessage>()
            .GetAllAsync();
            
        var recentChats = recentMessages
            .GroupBy(m => m.SenderEmail)
            .Select(g => new {
                Email = g.Key,
                Name = g.First(x => !x.IsAdminMessage).SenderName, // İlk mesajdan ismi al
                LastMessageDate = g.Max(x => x.SentDate),
                IsOnline = _userConnections.ContainsKey(g.Key)
            })
            .OrderByDescending(x => x.LastMessageDate)
            .Take(20)
            .Cast<object>()
            .ToList();
            
        return recentChats;
    }

    public async Task JoinSupportGroup()
    {
        if (Context.User != null && Context.User.IsInRole("Admin"))
        {
            Serilog.Log.Information("Admin {User} joined Support group", Context.User.Identity?.Name);
            await Groups.AddToGroupAsync(Context.ConnectionId, "Support");
            
            // Mevcut online kullanıcıları admine bildir
            foreach (var conn in _userConnections)
            {
                await Clients.Caller.SendAsync("UserStatusChanged", conn.Key, conn.Value, true);
            }
        }
    }

    // Sohbeti tamamen silme (Admin yetkisiyle)
    public async Task DeleteChat(string email)
    {
        if (Context.User != null && Context.User.IsInRole("Admin"))
        {
            var messages = await _unitOfWork.GetRepository<ChatMessage>()
                .GetByFilterAsync(m => m.SenderEmail == email);
            
            foreach (var msg in messages)
            {
                _unitOfWork.GetRepository<ChatMessage>().Delete(msg);
            }
            await _unitOfWork.SaveAsync();

            Serilog.Log.Information("Chat for {Email} deleted by admin", email);

            // Admine silindi bilgisini gönder
            await Clients.Group("Support").SendAsync("ChatDeleted", email);

            // Kullanıcıya (eğer online ise) silindi bilgisini gönder
            if (_userConnections.TryGetValue(email, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ChatDeletedByAdmin");
            }
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        var isAdmin = Context.User?.IsInRole("Admin") ?? false;
        
        if (!isAdmin && userName != "Anonymous")
        {
            _userConnections[userName] = Context.ConnectionId;
            await Clients.Group("Support").SendAsync("UserStatusChanged", userName, Context.ConnectionId, true);
        }

        if (isAdmin)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Support");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        if (!string.IsNullOrEmpty(userName) && _userConnections.TryRemove(userName, out _))
        {
            await Clients.Group("Support").SendAsync("UserStatusChanged", userName, null, false);
        }
        await base.OnDisconnectedAsync(exception);
    }
}
