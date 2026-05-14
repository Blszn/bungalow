using System.ComponentModel.DataAnnotations;

namespace Bungalov.WebUI.Models;

public class MakeReservationViewModel
{
    public int BungalowId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string BungalowName { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public List<GuestViewModel> Guests { get; set; } = new();
}

public class GuestViewModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string IdentityNumber { get; set; } = string.Empty;
}
