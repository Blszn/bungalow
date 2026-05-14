using Bungalov.Business.Interfaces;
using Bungalov.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bungalov.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IBungalowService _bungalowService;
    private readonly IReservationService _reservationService;
    private readonly ICategoryService _categoryService;
    private readonly IReviewService _reviewService;

    public AdminController(IBungalowService bungalowService, IReservationService reservationService, ICategoryService categoryService, IReviewService reviewService)
    {
        _bungalowService = bungalowService;
        _reservationService = reservationService;
        _categoryService = categoryService;
        _reviewService = reviewService;
    }

    public async Task<IActionResult> Index()
    {
        var bungalows = await _bungalowService.GetAllBungalowsAsync();
        var reservations = await _reservationService.GetReservationsWithDetailsAsync();
        var categories = await _categoryService.GetAllCategoriesAsync();
        var reviews = await _reviewService.GetAllReviewsWithDetailsAsync();

        var viewModel = new AdminDashboardViewModel
        {
            TotalBungalows = bungalows.Count,
            TotalCategories = categories.Count,
            TotalReservations = reservations.Count(r => !r.IsBlockedByAdmin),
            TotalEarnings = reservations.Where(r => r.IsPaid).Sum(r => r.TotalPrice),
            RecentReservations = reservations
                .Where(r => !r.IsBlockedByAdmin)
                .OrderByDescending(r => r.CreatedDate)
                .Take(10)
                .ToList(),
            CategoryStats = categories.Select(c => new CategoryStatViewModel
            {
                CategoryName = c.CategoryName,
                BungalowCount = bungalows.Count(b => b.CategoryId == c.Id)
            }).ToList(),
            TotalReviews = reviews.Count,
            AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
            RecentReviews = reviews
                .OrderByDescending(r => r.CreatedDate)
                .Take(5)
                .ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Reviews()
    {
        var reviews = await _reviewService.GetAllReviewsWithDetailsAsync();
        var bungalows = await _bungalowService.GetAllBungalowsAsync();

        var viewModel = new AdminReviewsViewModel
        {
            TotalReviews = reviews.Count,
            AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
            AllReviews = reviews.OrderByDescending(r => r.CreatedDate).ToList(),
            RatingDistribution = reviews.GroupBy(r => r.Rating)
                .ToDictionary(g => g.Key, g => g.Count()),
            BungalowRatings = bungalows.Select(b => new BungalowRatingViewModel
            {
                BungalowName = b.Name,
                ReviewCount = reviews.Count(r => r.BungalowId == b.Id),
                AverageRating = reviews.Where(r => r.BungalowId == b.Id).Any() 
                    ? reviews.Where(r => r.BungalowId == b.Id).Average(r => r.Rating) 
                    : 0
            }).OrderByDescending(x => x.AverageRating).ToList()
        };

        // Fill missing star counts in distribution
        for (int i = 1; i <= 5; i++)
        {
            if (!viewModel.RatingDistribution.ContainsKey(i))
                viewModel.RatingDistribution[i] = 0;
        }

        return View(viewModel);
    }

    public IActionResult Chat()
    {
        return View();
    }
}
