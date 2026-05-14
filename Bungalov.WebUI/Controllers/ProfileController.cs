using Bungalov.Business.Interfaces;
using Bungalov.Core.Varliklar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bungalov.WebUI.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IReservationService _reservationService;

    public ProfileController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IReservationService reservationService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _reservationService = reservationService;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var reservations = await _reservationService.GetReservationsByUserIdAsync(user.Id);
        
        ViewBag.FullName = user.FirstName + " " + user.LastName;
        ViewBag.Email = user.Email;
        ViewBag.PhoneNumber = user.PhoneNumber;
        ViewBag.Address = user.Address;

        return View(reservations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(string email, string phoneNumber)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false, message = "Kullanıcı bulunamadı." });

        user.Email = email;
        user.UserName = email;
        user.PhoneNumber = phoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            return Json(new { success = true, message = "Profil başarıyla güncellendi." });
        }

        var errors = string.Join("<br>", result.Errors.Select(e => e.Description));
        return Json(new { success = false, message = errors });
    }
}
