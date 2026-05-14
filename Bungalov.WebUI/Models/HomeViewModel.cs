using Bungalov.Core.Varliklar;

namespace Bungalov.WebUI.Models;

public class HomeViewModel
{
    public List<Category> Categories { get; set; } = new();
    public List<Bungalow> FeaturedBungalows { get; set; } = new();
    public List<Reservation> PendingReviews { get; set; } = new();
}
