using Bungalov.Business.Interfaces;
using Bungalov.Core.Varliklar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bungalov.WebUI.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly IReservationService _reservationService;
    private readonly IBungalowService _bungalowService;
    private readonly UserManager<AppUser> _userManager;

    public ReviewController(IReviewService reviewService, IReservationService reservationService, IBungalowService bungalowService, UserManager<AppUser> userManager)
    {
        _reviewService = reviewService;
        _reservationService = reservationService;
        _bungalowService = bungalowService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int bungalowId, int reservationId)
    {
        var bungalow = await _bungalowService.GetBungalowByIdAsync(bungalowId);
        if (bungalow == null) return NotFound();

        var reservation = await _reservationService.GetReservationByIdAsync(reservationId);
        if (reservation == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        // Güvenlik Kontrolleri
        if (reservation.AppUserId != user.Id)
        {
            return Forbid(); // Başkasının rezervasyonuna yorum yapamaz
        }

        /* TEST İÇİN GEÇİCİ OLARAK KALDIRILDI
        if (reservation.CheckOutDate > DateTime.UtcNow)
        {
            TempData["Error"] = "Değerlendirme yapabilmek için konaklamanızın tamamlanmış olması gerekmektedir.";
            return RedirectToAction("Index", "Profile");
        }
        */

        // Check if already reviewed
        var existingReview = await _reviewService.GetReviewByReservationIdAsync(reservationId);
        if (existingReview != null)
        {
            TempData["Info"] = "Bu konaklama için zaten bir değerlendirme yapmışsınız.";
            return RedirectToAction("Index", "Profile");
        }

        ViewBag.BungalowName = bungalow.Name;
        ViewBag.BungalowId = bungalowId;
        ViewBag.ReservationId = reservationId;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int bungalowId, int reservationId, int rating, string comment)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var review = new Review
        {
            BungalowId = bungalowId,
            ReservationId = reservationId,
            AppUserId = user.Id,
            Rating = rating,
            Comment = comment,
            CreatedDate = DateTime.UtcNow
        };

        await _reviewService.AddReviewAsync(review);
        TempData["Success"] = "Değerlendirmeniz başarıyla kaydedildi. Teşekkür ederiz!";
        
        return RedirectToAction("Index", "Profile");
    }

    [HttpGet]
    public async Task<IActionResult> QuickTest()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var userReservations = await _reservationService.GetReservationsByUserIdAsync(user.Id);
        if (!userReservations.Any(r => r.IsPaid))
        {
            TempData["Error"] = "Test için en az bir ödenmiş rezervasyonunuz olmalı.";
            return RedirectToAction("Index", "Profile");
        }

        // Önce şu an bakılan bungalovun rezervasyonu var mı ona bak (Referer'dan ID çekmeye çalış)
        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && referer.Contains("/Bungalow/Details/"))
        {
            var parts = referer.Split('/');
            if (int.TryParse(parts.Last().Split('?')[0], out int bungalowId))
            {
                var contextReservation = userReservations
                    .Where(r => r.BungalowId == bungalowId && r.IsPaid)
                    .OrderByDescending(r => r.CreatedDate)
                    .FirstOrDefault();

                if (contextReservation != null)
                {
                    return RedirectToAction("Index", new { bungalowId = contextReservation.BungalowId, reservationId = contextReservation.Id });
                }
            }
        }

        // Yoksa en son yapılan ödenmiş rezervasyonu bul
        var latestReservation = userReservations
            .Where(r => r.IsPaid)
            .OrderByDescending(r => r.CreatedDate)
            .FirstOrDefault();

        return RedirectToAction("Index", new { bungalowId = latestReservation!.BungalowId, reservationId = latestReservation.Id });
    }
}
