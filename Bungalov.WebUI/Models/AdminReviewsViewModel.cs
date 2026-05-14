using System.Collections.Generic;
using Bungalov.Core.Varliklar;

namespace Bungalov.WebUI.Models;

public class AdminReviewsViewModel
{
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; } = new();
    public List<Review> AllReviews { get; set; } = new();
    public List<BungalowRatingViewModel> BungalowRatings { get; set; } = new();
}

public class BungalowRatingViewModel
{
    public string BungalowName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}
