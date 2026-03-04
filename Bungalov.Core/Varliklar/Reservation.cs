using System;

namespace Bungalov.Core.Varliklar;

public class Reservation : BaseEntity
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public int AppUserId { get; set; }
    public int BungalowId { get; set; }
    public Bungalow Bungalow { get; set; } = null!;
}