using System.Collections.Generic;

namespace Bungalov.Core.Varliklar;

public class Bungalow : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool HasJacuzzi { get; set; }
    public bool HasPool { get; set; }
    public bool IsWifiAvailable { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}