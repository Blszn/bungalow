using System.Collections.Generic;
    
namespace Bungalov.Core.Varliklar;

public class Bungalow : BaseEntity
{
    // ── Temel Bilgiler ──────────────────────────────────────────────
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }

    // ── Konum Bilgileri ─────────────────────────────────────────────
    public string Location { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // ── Fiziksel Bilgiler ────────────────────────────────────────────
    public int? SizeM2 { get; set; }
    public int MinNights { get; set; } = 1;
    
    // Giriş/Çıkış saatleri formdan kaldırılsa da veritabanında saklanabilir (Default: 14:00 - 11:00)
    public string CheckInTime { get; set; } = "14:00";
    public string CheckOutTime { get; set; } = "11:00";

    // ── İlişkiler ────────────────────────────────────────────────────
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    // Dinamik Olanaklar
    public ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
    
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<BungalowImage> Images { get; set; } = new List<BungalowImage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}