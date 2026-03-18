using Bungalov.Business.Interfaces;
using Bungalov.Core.Varliklar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bungalov.WebUI.Controllers;

public class BungalowController : Controller
{
    private readonly IBungalowService _bungalowService;
    private readonly ICategoryService _categoryService;
    private readonly IWebHostEnvironment _env;

    public BungalowController(IBungalowService bungalowService, ICategoryService categoryService, IWebHostEnvironment env)
    {
        _bungalowService = bungalowService;
        _categoryService = categoryService;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var bungalows = await _bungalowService.GetAllBungalowsAsync();
        return View(bungalows);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Bungalow bungalow, List<IFormFile>? newImages)
    {
        ModelState.Remove("Category");
        // Images içindeki Navigation property'lerin eksik olmasından dolayı ModelState hata verebilir.
        var dict = ModelState.Keys.Where(k => k.StartsWith("Images[") && k.EndsWith(".Bungalow")).ToList();
        foreach (var key in dict) ModelState.Remove(key);

        if (ModelState.IsValid)
        {
            if (newImages != null && newImages.Count > 0)
            {
                var location = Path.Combine(_env.WebRootPath, "images", "bungalows");
                if (!Directory.Exists(location))
                {
                    Directory.CreateDirectory(location);
                }

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

                        bungalow.Images.Add(new BungalowImage
                        {
                            ImageUrl = "/images/bungalows/" + newImageName
                        });
                    }
                }
            }

            await _bungalowService.AddBungalowAsync(bungalow);
            return RedirectToAction(nameof(Index));
        }

        var categories = await _categoryService.GetAllCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
        return View(bungalow);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var bungalow = await _bungalowService.GetBungalowByIdAsync(id);
        if (bungalow == null) return NotFound();

        var categories = await _categoryService.GetAllCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", bungalow.CategoryId);
        return View(bungalow);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Bungalow bungalow, List<IFormFile>? newImages)
    {
        ModelState.Remove("Category");
        var dict = ModelState.Keys.Where(k => k.StartsWith("Images[") && k.EndsWith(".Bungalow")).ToList();
        foreach (var key in dict) ModelState.Remove(key);

        if (ModelState.IsValid)
        {
            var existingBungalow = await _bungalowService.GetBungalowByIdAsync(bungalow.Id);
            if (existingBungalow == null) return NotFound();

            // Sadece formdan gelen, değişen verileri güncelleyelim
            existingBungalow.Name = bungalow.Name;
            existingBungalow.Description = bungalow.Description;
            existingBungalow.PricePerNight = bungalow.PricePerNight;
            existingBungalow.Capacity = bungalow.Capacity;
            existingBungalow.Location = bungalow.Location;
            existingBungalow.HasJacuzzi = bungalow.HasJacuzzi;
            existingBungalow.HasPool = bungalow.HasPool;
            existingBungalow.IsWifiAvailable = bungalow.IsWifiAvailable;
            existingBungalow.CategoryId = bungalow.CategoryId;

            if (newImages != null && newImages.Count > 0)
            {
                var location = Path.Combine(_env.WebRootPath, "images", "bungalows");
                if (!Directory.Exists(location))
                {
                    Directory.CreateDirectory(location);
                }

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
                // Fiziksel dosyayı silme
                var path = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                bungalow.Images.Remove(image);
                await _bungalowService.UpdateBungalowAsync(bungalow);
            }
        }
        return RedirectToAction(nameof(Edit), new { id = bungalowId });
    }
}
