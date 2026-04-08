using Bungalov.Business.Interfaces;
using Bungalov.Core.Varliklar;
using Microsoft.AspNetCore.Mvc;

namespace Bungalov.WebUI.Controllers;

public class ReservationController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly IBungalowService _bungalowService;
    private readonly IEmailService _emailService;

    public ReservationController(IReservationService reservationService, IBungalowService bungalowService, IEmailService emailService)
    {
        _reservationService = reservationService;
        _bungalowService = bungalowService;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<JsonResult> GetBookedDates(int bungalowId)
    {
        var dates = await _reservationService.GetBookedDatesAsync(bungalowId);
        // FullCalendar'ın beklediği formatta (YYYY-MM-DD) veya basit liste
        return Json(dates.Select(d => d.ToString("yyyy-MM-dd")));
    }

    [HttpPost]
    public async Task<IActionResult> MakeReservation(int bungalowId, DateTime checkIn, DateTime checkOut)
    {
        if (checkIn >= checkOut)
        {
            return Json(new { success = false, message = "Çıkış tarihi giriş tarihinden sonra olmalıdır." });
        }

        if (checkIn < DateTime.Now.Date)
        {
            return Json(new { success = false, message = "Geçmiş tarihlere rezervasyon yapılamaz." });
        }

        var isAvailable = await _reservationService.IsAvailableAsync(bungalowId, checkIn, checkOut);
        if (!isAvailable)
        {
            return Json(new { success = false, message = "Seçilen tarihler arasında bungalov doludur." });
        }

        var bungalow = await _bungalowService.GetBungalowByIdAsync(bungalowId);
        if (bungalow == null) return NotFound();

        // Toplam fiyat hesaplama
        var totalDays = (checkOut - checkIn).Days;
        var totalPrice = totalDays * bungalow.PricePerNight;

        var reservation = new Reservation
        {
            BungalowId = bungalowId,
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            TotalPrice = totalPrice,
            AppUserId = 1 // Şimdilik statik, auth sisteminden sonra güncellenebilir.
        };

        await _reservationService.AddReservationAsync(reservation);

        // E-posta bildirimi (Simülasyon)
        await _emailService.SendEmailAsync("user@example.com", "Rezervasyon Onayı", 
            $"{bungalow.Name} için {checkIn:dd.MM.yyyy} - {checkOut:dd.MM.yyyy} tarihleri arasındaki rezervasyonunuz başarıyla oluşturulmuştur. Toplam Tutar: ₺{totalPrice}");

        return Json(new { success = true, message = "Rezervasyonunuz başarıyla oluşturuldu! E-posta onayınız gönderildi.", totalPrice = totalPrice });
    }
}
