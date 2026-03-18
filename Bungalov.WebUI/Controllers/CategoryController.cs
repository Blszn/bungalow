using Bungalov.Business.Interfaces;
using Bungalov.Core.Varliklar;
using Microsoft.AspNetCore.Mvc;

namespace Bungalov.WebUI.Controllers;

public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IWebHostEnvironment _env;

    public CategoryController(ICategoryService categoryService, IWebHostEnvironment env)
    {
        _categoryService = categoryService;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return View(categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName);
                var newImageName = Guid.NewGuid().ToString() + extension;
                var location = Path.Combine(_env.WebRootPath, "images", "categories");
                
                if (!Directory.Exists(location))
                {
                    Directory.CreateDirectory(location);
                }

                var path = Path.Combine(location, newImageName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                
                category.ImageUrl = "/images/categories/" + newImageName;
            }

            await _categoryService.AddCategoryAsync(category);
            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();
        
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Category category, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            // Veritabanındaki eski kaydı çekerek silinen verileri koruyoruz (CreatedDate vb.)
            var existingCategory = await _categoryService.GetCategoryByIdAsync(category.Id);
            if (existingCategory == null) return NotFound();

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.Description = category.Description;

            if (imageFile != null && imageFile.Length > 0)
            {
                // Eski resmi silebiliriz ama basitlik adına şimdilik sadece yenisini atıyoruz
                var extension = Path.GetExtension(imageFile.FileName);
                var newImageName = Guid.NewGuid().ToString() + extension;
                var location = Path.Combine(_env.WebRootPath, "images", "categories");
                
                if (!Directory.Exists(location))
                {
                    Directory.CreateDirectory(location);
                }

                var path = Path.Combine(location, newImageName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                
                existingCategory.ImageUrl = "/images/categories/" + newImageName;
            }

            await _categoryService.UpdateCategoryAsync(existingCategory);
            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
