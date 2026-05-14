using System;
using System.Collections.Generic;
using Bungalov.Core.Varliklar;

namespace Bungalov.WebUI.Models;

public class AdminDashboardViewModel
{
    public int TotalBungalows { get; set; }
    public int TotalCategories { get; set; }
    public int TotalReservations { get; set; }
    public decimal TotalEarnings { get; set; }
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    
    public List<Reservation> RecentReservations { get; set; } = new();
    public List<CategoryStatViewModel> CategoryStats { get; set; } = new();
    public List<Review> RecentReviews { get; set; } = new();
}

public class CategoryStatViewModel
{
    public string CategoryName { get; set; } = string.Empty;
    public int BungalowCount { get; set; }
}
