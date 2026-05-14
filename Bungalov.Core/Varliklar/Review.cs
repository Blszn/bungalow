using System;

namespace Bungalov.Core.Varliklar;

public class Review : BaseEntity
{
    public int Rating { get; set; } // 1-5
    public string Comment { get; set; } = string.Empty;
    
    public int BungalowId { get; set; }
    public Bungalow Bungalow { get; set; } = null!;
    
    public string AppUserId { get; set; } = string.Empty;
    public AppUser AppUser { get; set; } = null!;
    
    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;
}
