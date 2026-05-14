using Bungalov.Business.Interfaces;
using Bungalov.Core.Varliklar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Bungalov.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AmenityController : Controller
{
    private readonly IAmenityService _amenityService;

    public AmenityController(IAmenityService amenityService)
    {
        _amenityService = amenityService;
    }

    public async Task<IActionResult> Index()
    {
        var amenities = await _amenityService.GetAllAmenitiesAsync();
        return View(amenities);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Amenity amenity)
    {
        if (ModelState.IsValid)
        {
            await _amenityService.AddAmenityAsync(amenity);
            return RedirectToAction(nameof(Index));
        }
        return View(amenity);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var amenity = await _amenityService.GetAmenityByIdAsync(id);
        if (amenity == null) return NotFound();
        return View(amenity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Amenity amenity)
    {
        if (ModelState.IsValid)
        {
            await _amenityService.UpdateAmenityAsync(amenity);
            return RedirectToAction(nameof(Index));
        }
        return View(amenity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _amenityService.DeleteAmenityAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
