using Bungalov.Business.Interfaces;
using Bungalov.Core.Varliklar;
using Bungalov.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Bungalov.WebUI.Controllers;

public class ReservationController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly IBungalowService _bungalowService;
    private readonly IEmailService _emailService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IPaymentService _paymentService;

    public ReservationController(IReservationService reservationService, IBungalowService bungalowService, IEmailService emailService, UserManager<AppUser> userManager, IPaymentService paymentService)
    {
        _reservationService = reservationService;
        _bungalowService = bungalowService;
        _emailService = emailService;
        _userManager = userManager;
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<JsonResult> GetBookedDates(int bungalowId)
    {
        var dates = await _reservationService.GetBookedDatesAsync(bungalowId);
        // FullCalendar'ın beklediği formatta (YYYY-MM-DD) veya basit liste
        return Json(dates.Select(d => d.ToString("yyyy-MM-dd")));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GuestInfo(int bungalowId, DateTime checkIn, DateTime checkOut, int guestCount)
    {
        var bungalow = await _bungalowService.GetBungalowByIdAsync(bungalowId);
        if (bungalow == null) return NotFound();

        if (checkIn.Date >= checkOut.Date)
        {
            var errorMsg = $"Çıkış tarihi ({checkOut:dd.MM.yyyy}) giriş tarihinden ({checkIn:dd.MM.yyyy}) sonra olmalıdır.";
            return RedirectToAction("Details", "Bungalow", new { id = bungalowId, error = errorMsg });
        }

        if (checkIn.Date < DateTime.Now.Date)
            return RedirectToAction("Details", "Bungalow", new { id = bungalowId, error = "Geçmiş tarihlere rezervasyon yapılamaz." });

        // PostgreSQL UTC zorunluluğu
        var utcCheckIn = DateTime.SpecifyKind(checkIn, DateTimeKind.Utc);
        var utcCheckOut = DateTime.SpecifyKind(checkOut, DateTimeKind.Utc);

        var isAvailable = await _reservationService.IsAvailableAsync(bungalowId, utcCheckIn, utcCheckOut);
        if (!isAvailable)
            return RedirectToAction("Details", "Bungalow", new { id = bungalowId, error = "Seçilen tarihler arasında bungalov doludur." });

        var totalDays = (checkOut - checkIn).Days;
        var totalPrice = totalDays * bungalow.PricePerNight;

        var model = new MakeReservationViewModel
        {
            BungalowId = bungalowId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            GuestCount = guestCount, // Note: Need to add this to ViewModel if not exists or use ViewBag
            TotalPrice = totalPrice,
            BungalowName = bungalow.Name,
            PricePerNight = bungalow.PricePerNight
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateReservation([FromBody] MakeReservationViewModel model)
    {
        try 
        {
            if (model.CheckIn.Date >= model.CheckOut.Date)
            {
                var msg = $"Çıkış tarihi ({model.CheckOut:dd.MM.yyyy}) giriş tarihinden ({model.CheckIn:dd.MM.yyyy}) sonra olmalıdır. (POST)";
                return Json(new { success = false, message = msg });
            }

            // PostgreSQL UTC zorunluluğu
            var utcCheckIn = DateTime.SpecifyKind(model.CheckIn, DateTimeKind.Utc);
            var utcCheckOut = DateTime.SpecifyKind(model.CheckOut, DateTimeKind.Utc);

            var isAvailable = await _reservationService.IsAvailableAsync(model.BungalowId, utcCheckIn, utcCheckOut);
            if (!isAvailable)
                return Json(new { success = false, message = "Seçilen tarihler arasında bungalov doludur." });

            var bungalow = await _bungalowService.GetBungalowByIdAsync(model.BungalowId);
            if (bungalow == null) return NotFound();

            var totalDays = (utcCheckOut - utcCheckIn).Days;
            var totalPrice = totalDays * bungalow.PricePerNight;

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var reservation = new Reservation
            {
                BungalowId = model.BungalowId,
                CheckInDate = utcCheckIn,
                CheckOutDate = utcCheckOut,
                TotalPrice = totalPrice,
                AppUserId = user.Id,
                IsPaid = false,
                Note = "Ödeme bekleniyor",
                Guests = model.Guests.Select(g => new Guest 
                { 
                    FirstName = g.FirstName, 
                    LastName = g.LastName, 
                    IdentityNumber = g.IdentityNumber 
                }).ToList()
            };

            await _reservationService.AddReservationAsync(reservation);

            return Json(new { success = true, redirectUrl = Url.Action("Payment", new { id = reservation.Id }) });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Hata: " + ex.Message });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Payment(int id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        if (reservation == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null || reservation.AppUserId != user.Id) return Unauthorized();

        if (reservation.IsPaid) return RedirectToAction("Result", new { success = true, id = reservation.Id });

        var callbackUrl = Url.Action("PaymentCallback", "Reservation", new { id = reservation.Id }, Request.Scheme)!;
        var paymentInit = await _paymentService.InitializeCheckoutFormAsync(reservation.TotalPrice, reservation.BungalowId, user.Id, callbackUrl);

        if (paymentInit.Status == "success")
        {
            reservation.PaymentToken = paymentInit.Token;
            await _reservationService.UpdateReservationAsync(reservation);

            ViewBag.CheckoutFormContent = paymentInit.CheckoutFormContent;
            return View(reservation);
        }

        return RedirectToAction("Result", new { success = false, message = "Ödeme sistemi başlatılamadı: " + paymentInit.ErrorMessage });
    }

    [HttpPost]
    public async Task<IActionResult> PaymentCallback(int id, [FromForm] string token)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        if (reservation == null) return NotFound();

        var paymentResult = await _paymentService.RetrieveCheckoutFormResultAsync(token);

        if (paymentResult.Status == "success" && paymentResult.PaymentStatus == "SUCCESS")
        {
            if (paymentResult.Status == "success")
            {
                reservation.IsPaid = true;
                reservation.PaymentId = paymentResult.PaymentId;
                reservation.Note = "Ödeme onaylandı";
                await _reservationService.UpdateReservationAsync(reservation);

                // --- VOUCHER E-POSTASI GÖNDERİMİ ---
                var user = await _userManager.FindByIdAsync(reservation.AppUserId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var lat = reservation.Bungalow.Latitude?.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    var lon = reservation.Bungalow.Longitude?.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    
                    var googleMapsUrl = (lat != null && lon != null)
                        ? $"https://www.google.com/maps?q={lat},{lon}&z=15"
                        : "#";

                    string guestRows = "";
                    foreach (var guest in reservation.Guests)
                    {
                        guestRows += $"<li>{guest.FirstName} {guest.LastName}</li>";
                    }

                    var emailTemplate = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; border-radius: 10px; overflow: hidden;'>
                            <div style='background: #0d6efd; color: white; padding: 20px; text-align: center;'>
                                <h2>Rezervasyonunuz Onaylandı!</h2>
                                <p>Bungalov maceranız için her şey hazır.</p>
                            </div>
                            <div style='padding: 20px;'>
                                <p>Merhaba <strong>{user.UserName}</strong>,</p>
                                <p><strong>{reservation.Bungalow.Name}</strong> için yaptığınız rezervasyon başarıyla tamamlanmıştır.</p>
                                
                                <div style='background: #f8f9fa; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                                    <p style='margin: 0;'><strong>PNR:</strong> #RZV-{reservation.Id}</p>
                                    <p style='margin: 5px 0;'><strong>Giriş:</strong> {reservation.CheckInDate.ToLocalTime():dd MMM yyyy}</p>
                                    <p style='margin: 5px 0;'><strong>Çıkış:</strong> {reservation.CheckOutDate.ToLocalTime():dd MMM yyyy}</p>
                                    <p style='margin: 5px 0;'><strong>Toplam Tutar:</strong> ₺{reservation.TotalPrice:N2}</p>
                                </div>

                                <h3>Konaklayacak Misafirler</h3>
                                <ul>{guestRows}</ul>

                                <div style='text-align: center; margin-top: 30px;'>
                                    <a href='{googleMapsUrl}' style='background: #198754; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Bungalov Konumuna Git</a>
                                </div>
                            </div>
                            <div style='background: #f1f1f1; padding: 15px; text-align: center; font-size: 12px; color: #777;'>
                                <p>Bu bir otomatik bilgilendirme e-postasıdır. Lütfen yanıtlamayınız.</p>
                            </div>
                        </div>
                    ";

                    await _emailService.SendEmailAsync(user.Email, "Rezervasyonunuz Onaylandı - Rivora Keyfi Başlıyor!", emailTemplate);
                }

                ViewBag.Success = true;
                return View("Result", reservation);
            }
        }

        // Ödeme başarısız
        reservation.Note = "Ödeme Başarısız: " + paymentResult.ErrorMessage;
        await _reservationService.UpdateReservationAsync(reservation);

        return RedirectToAction("Result", new { success = false, message = paymentResult.ErrorMessage });
    }

    [HttpGet]
    public async Task<IActionResult> Result(bool success, string? message, int? id)
    {
        ViewBag.Success = success;
        ViewBag.Message = message;
        
        if (id.HasValue)
        {
            var res = await _reservationService.GetReservationByIdAsync(id.Value);
            return View(res);
        }
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelReservation(int id)
    {
        try
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
                return Json(new { success = false, message = "Rezervasyon bulunamadı." });

            var user = await _userManager.GetUserAsync(User);
            if (user == null || (reservation.AppUserId != user.Id && !User.IsInRole("Admin")))
                return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });

            if (reservation.CheckInDate <= DateTime.UtcNow)
                return Json(new { success = false, message = "Geçmiş veya başlamış rezervasyonlar iptal edilemez." });

            // Bilgilendirme e-postası için bilgileri sakla
            var bungalowName = reservation.Bungalow.Name;
            var checkIn = reservation.CheckInDate;
            var checkOut = reservation.CheckOutDate;
            var userEmail = user.Email;

            await _reservationService.DeleteReservationAsync(id);

            // E-posta gönder
            if (!string.IsNullOrEmpty(userEmail))
            {
                var emailBody = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; border-radius: 10px; padding: 20px;'>
                        <h2 style='color: #dc3545;'>Rezervasyon İptal Edildi</h2>
                        <p>Sayın {user.FirstName} {user.LastName},</p>
                        <p><strong>{bungalowName}</strong> için yaptığınız rezervasyon isteğiniz üzerine iptal edilmiştir.</p>
                        <div style='background: #f8f9fa; padding: 15px; border-radius: 8px;'>
                            <p style='margin: 0;'><strong>İptal Edilen Tarihler:</strong> {checkIn.ToLocalTime():dd MMM} - {checkOut.ToLocalTime():dd MMM yyyy}</p>
                        </div>
                        <p style='margin-top: 20px;'>İade süreçleri ile ilgili ödeme sağlayıcınızla (iZico) iletişime geçebilirsiniz.</p>
                    </div>";

                await _emailService.SendEmailAsync(userEmail, "Rezervasyon İptali - " + bungalowName, emailBody);
            }

            return Json(new { success = true, message = "Rezervasyon başarıyla iptal edildi." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
        }
    }
}
