using Bungalov.Business.Interfaces;
using Bungalov.Core.Varliklar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bungalov.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminReservationController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly IBungalowService _bungalowService;
    private readonly UserManager<AppUser> _userManager;

    public AdminReservationController(IReservationService reservationService, IBungalowService bungalowService, UserManager<AppUser> userManager)
    {
        _reservationService = reservationService;
        _bungalowService = bungalowService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Calendar(int bungalowId)
    {
        var bungalows = await _bungalowService.GetBungalowsByFilterAsync(b => b.Id == bungalowId);
        var bungalow = bungalows.FirstOrDefault();
        if (bungalow == null) return NotFound();

        ViewBag.BungalowName = bungalow.Name;
        ViewBag.BungalowId = bungalow.Id;

        // Bloklanmış tarihleri ayrıca listelemek isteyebiliriz
        var allReservations = await _reservationService.GetAllReservationsAsync();
        var blockedReservations = allReservations
            .Where(r => r.BungalowId == bungalowId && r.IsBlockedByAdmin)
            .OrderByDescending(r => r.CheckInDate)
            .ToList();

        return View(blockedReservations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BlockDates(int bungalowId, DateTime checkIn, DateTime checkOut, string? note)
    {
        try 
        {
            if (checkIn >= checkOut)
            {
                return Json(new { success = false, message = "Çıkış tarihi giriş tarihinden sonra olmalıdır." });
            }

            // PostgreSQL UTC zorunluluğu için tarihleri işaretle
            var utcCheckIn = DateTime.SpecifyKind(checkIn, DateTimeKind.Utc);
            var utcCheckOut = DateTime.SpecifyKind(checkOut, DateTimeKind.Utc);

            var isAvailable = await _reservationService.IsAvailableAsync(bungalowId, utcCheckIn, utcCheckOut);
            if (!isAvailable)
            {
                return Json(new { success = false, message = "Seçilen tarihlerde mevcut bir rezervasyon veya blok bulunmaktadır." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var reservation = new Reservation
            {
                BungalowId = bungalowId,
                CheckInDate = utcCheckIn,
                CheckOutDate = utcCheckOut,
                IsBlockedByAdmin = true,
                Note = note ?? "Admin tarafından kapatıldı",
                AppUserId = userId,
                TotalPrice = 0
            };

            await _reservationService.AddReservationAsync(reservation);
            return Json(new { success = true, message = "Tarihler başarıyla kapatıldı." });
        }
        catch (Exception ex)
        {
            return Json(new { 
                success = false, 
                message = "Sunucu Hatası: " + ex.Message,
                detail = ex.InnerException?.Message 
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnblockDates(int id)
    {
        await _reservationService.DeleteReservationAsync(id);
        return Json(new { success = true, message = "Blok başarıyla kaldırıldı." });
    }
}
