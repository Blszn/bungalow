using Bungalov.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bungalov.WebUI.ViewComponents;

public class CategoryListViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;

    public CategoryListViewComponent(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int? selectedCategoryId)
    {
        ViewBag.SelectedCategoryId = selectedCategoryId;
        var categories = await _categoryService.GetAllCategoriesAsync();
        return View(categories);
    }
}
