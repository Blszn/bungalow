using Bungalov.Business.Interfaces;
using Bungalov.Core.Varliklar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bungalov.WebUI.Controllers;

public class BungalowController : Controller
{
    private readonly IBungalowService _bungalowService;
    private readonly ICategoryService _categoryService;
    private readonly IAmenityService _amenityService;
    private readonly IWebHostEnvironment _env;

    public BungalowController(IBungalowService bungalowService, ICategoryService categoryService, IAmenityService amenityService, IWebHostEnvironment env)
    {
        _bungalowService = bungalowService;
        _categoryService = categoryService;
        _amenityService = amenityService;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, int? categoryId, int? minCapacity, int[]? amenityIds)
    {
        // Temel filtreleme
        var bungalows = await _bungalowService.GetBungalowsByFilterAsync(b =>
            (string.IsNullOrEmpty(search) || b.Name.Contains(search) || b.Location.Contains(search)) &&
            (!categoryId.HasValue || b.CategoryId == categoryId.Value) &&
            (!minCapacity.HasValue || b.Capacity >= minCapacity.Value));

        // Özellik filtrelemesi (Seçilen tüm özelliklere sahip olanları getir - AND mantığı)
        if (amenityIds != null && amenityIds.Any())
        {
            bungalows = bungalows.Where(b => amenityIds.All(id => b.Amenities.Any(a => a.Id == id))).ToList();
        }

        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.MinCapacity = minCapacity;
        ViewBag.SelectedAmenityIds = amenityIds ?? Array.Empty<int>();
        
        // Sidebar için tüm aktif özellikleri getir
        ViewBag.AllAmenities = await _amenityService.GetAllAmenitiesAsync();

        return View(bungalows);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var bungalow = await _bungalowService.GetBungalowByIdAsync(id);
        if (bungalow == null) return NotFound();

        return View(bungalow);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        var amenities = await _amenityService.GetAllAmenitiesAsync();
        
        ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
        ViewBag.Amenities = amenities;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Bungalow bungalow, List<IFormFile>? newImages, int[] selectedAmenityIds)
    {
        ModelState.Remove("Category");
        var dict = ModelState.Keys.Where(k => (k.StartsWith("Images[") && k.EndsWith(".Bungalow")) || k == "Amenities").ToList();
        foreach (var key in dict) ModelState.Remove(key);

        if (ModelState.IsValid)
        {
            // Seçilen özellikleri bağla
            if (selectedAmenityIds != null)
            {
                foreach (var id in selectedAmenityIds)
                {
                    var amenity = await _amenityService.GetAmenityByIdAsync(id);
                    if (amenity != null) bungalow.Amenities.Add(amenity);
                }
            }

            if (newImages != null && newImages.Count > 0)
            {
                var location = Path.Combine(_env.WebRootPath, "images", "bungalows");
                if (!Directory.Exists(location)) Directory.CreateDirectory(location);

                foreach (var image in newImages)
                {
                    if (image.Length > 0)
                    {
                        var extension = Path.GetExtension(image.FileName);
                        var newImageName = Guid.NewGuid().ToString() + extension;
                        var path = Path.Combine(location, newImageName);
                        
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        bungalow.Images.Add(new BungalowImage { ImageUrl = "/images/bungalows/" + newImageName });
                    }
                }
            }

            await _bungalowService.AddBungalowAsync(bungalow);
            return RedirectToAction(nameof(Index));
        }

        var categories = await _categoryService.GetAllCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
        ViewBag.Amenities = await _amenityService.GetAllAmenitiesAsync();
        return View(bungalow);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        // Bungalow'u özellikleri ile birlikte çekmek önemli
        var bungalows = await _bungalowService.GetBungalowsByFilterAsync(b => b.Id == id);
        var bungalow = bungalows.FirstOrDefault();
        if (bungalow == null) return NotFound();

        // UnitOfWork tabanlı repository'miz Many-to-Many ilişkisini (Amenities) otomatik include etmiyor olabilir.
        // Bu yüzden eğer bungalow.Amenities boş geliyorsa extra sorgu gerekebilir.
        // Şimdilik repository'nin Include desteği olduğunu varsayıyorum (GetAllAsync içinde vardı).

        var categories = await _categoryService.GetAllCategoriesAsync();
        var amenities = await _amenityService.GetAllAmenitiesAsync();

        ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", bungalow.CategoryId);
        ViewBag.Amenities = amenities;
        return View(bungalow);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Bungalow bungalow, List<IFormFile>? newImages, int[] selectedAmenityIds)
    {
        ModelState.Remove("Category");
        var dict = ModelState.Keys.Where(k => (k.StartsWith("Images[") && k.EndsWith(".Bungalow")) || k == "Amenities").ToList();
        foreach (var key in dict) ModelState.Remove(key);

        if (ModelState.IsValid)
        {
            // Bungalow'u tüm ilişkileriyle çek (Amenities dahil)
            var existingBungalow = await _bungalowService.GetBungalowByIdAsync(bungalow.Id);
            if (existingBungalow == null) return NotFound();

            existingBungalow.Name = bungalow.Name;
            existingBungalow.Description = bungalow.Description;
            existingBungalow.PricePerNight = bungalow.PricePerNight;
            existingBungalow.Capacity = bungalow.Capacity;
            existingBungalow.Location = bungalow.Location;
            existingBungalow.Province = bungalow.Province;
            existingBungalow.District = bungalow.District;
            existingBungalow.Neighborhood = bungalow.Neighborhood;
            existingBungalow.Address = bungalow.Address;
            existingBungalow.SizeM2 = bungalow.SizeM2;
            existingBungalow.MinNights = bungalow.MinNights;
            existingBungalow.CategoryId = bungalow.CategoryId;

            // Özellikleri güncelle (Önce temizle sonra yeni seçilenleri ekle)
            existingBungalow.Amenities.Clear();
            if (selectedAmenityIds != null)
            {
                foreach (var id in selectedAmenityIds)
                {
                    var amenity = await _amenityService.GetAmenityByIdAsync(id);
                    if (amenity != null) existingBungalow.Amenities.Add(amenity);
                }
            }

            if (newImages != null && newImages.Count > 0)
            {
                var location = Path.Combine(_env.WebRootPath, "images", "bungalows");
                if (!Directory.Exists(location)) Directory.CreateDirectory(location);

                foreach (var image in newImages)
                {
                    if (image.Length > 0)
                    {
                        var extension = Path.GetExtension(image.FileName);
                        var newImageName = Guid.NewGuid().ToString() + extension;
                        var path = Path.Combine(location, newImageName);
                        
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        existingBungalow.Images.Add(new BungalowImage
                        {
                            ImageUrl = "/images/bungalows/" + newImageName,
                            BungalowId = existingBungalow.Id
                        });
                    }
                }
            }

            await _bungalowService.UpdateBungalowAsync(existingBungalow);
            return RedirectToAction(nameof(Index));
        }

        var categories = await _categoryService.GetAllCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", bungalow.CategoryId);
        ViewBag.Amenities = await _amenityService.GetAllAmenitiesAsync();
        return View(bungalow);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _bungalowService.DeleteBungalowAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int imageId, int bungalowId)
    {
        var bungalow = await _bungalowService.GetBungalowByIdAsync(bungalowId);
        if (bungalow != null && bungalow.Images != null)
        {
            var image = bungalow.Images.FirstOrDefault(i => i.Id == imageId);
            if (image != null)
            {
                var path = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

                bungalow.Images.Remove(image);
                await _bungalowService.UpdateBungalowAsync(bungalow);
            }
        }
        return RedirectToAction(nameof(Edit), new { id = bungalowId });
    }
}
