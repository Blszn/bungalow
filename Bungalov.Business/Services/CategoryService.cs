using Bungalov.Business.Interfaces;
using Bungalov.Core.Interfaces;
using Bungalov.Core.Varliklar;

namespace Bungalov.Business.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddCategoryAsync(Category category)
    {
        await _unitOfWork.GetRepository<Category>().AddAsync(category);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
        if (category != null)
        {
            _unitOfWork.GetRepository<Category>().Delete(category);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _unitOfWork.GetRepository<Category>().GetAllAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        _unitOfWork.GetRepository<Category>().Update(category);
        await _unitOfWork.SaveAsync();
    }
}
