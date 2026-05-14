using System;

namespace Bungalov.Core.Varliklar;

public class Reservation : BaseEntity
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string AppUserId { get; set; } = string.Empty;
    public AppUser AppUser { get; set; } = null!;
    public int BungalowId { get; set; }
    public Bungalow Bungalow { get; set; } = null!;
    public ICollection<Guest> Guests { get; set; } = new List<Guest>();
    public bool IsBlockedByAdmin { get; set; } = false;
    public string? Note { get; set; }

    // ── Ödeme Bilgileri ─────────────────────────────────────────────
    public bool IsPaid { get; set; } = false;
    public string? PaymentId { get; set; }
    public string? PaymentToken { get; set; }
}