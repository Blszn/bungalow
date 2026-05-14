using Bungalov.Business.Interfaces;
using Bungalov.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Bungalov.Core.Varliklar;
using Microsoft.AspNetCore.Identity;

namespace Bungalov.WebUI.Controllers;

public class HomeController : Controller
{
    private readonly IBungalowService _bungalowService;
    private readonly ICategoryService _categoryService;
    private readonly IReviewService _reviewService;
    private readonly UserManager<AppUser> _userManager;

    public HomeController(IBungalowService bungalowService, ICategoryService categoryService, IReviewService reviewService, UserManager<AppUser> userManager)
    {
        _bungalowService = bungalowService;
        _categoryService = categoryService;
        _reviewService = reviewService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        var allBungalows = await _bungalowService.GetAllBungalowsAsync();
        
        var featured = allBungalows
            .OrderByDescending(b => b.Id)
            .Take(4)
            .ToList();

        var pendingReviews = new List<Reservation>();
        if (User.Identity!.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                pendingReviews = await _reviewService.GetPendingReviewsByUserIdAsync(user.Id);
            }
        }

        var viewModel = new HomeViewModel
        {
            Categories = categories,
            FeaturedBungalows = featured,
            PendingReviews = pendingReviews
        };

        return View(viewModel);
    }
}
