using Bungalov.Business.Interfaces;
using Bungalov.Core.Interfaces;
using Bungalov.Core.Varliklar;
using System.Linq.Expressions;

namespace Bungalov.Business.Services;

public class BungalowService : IBungalowService
{
    private readonly IUnitOfWork _unitOfWork;

    public BungalowService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddBungalowAsync(Bungalow bungalow)
    {
        await _unitOfWork.GetRepository<Bungalow>().AddAsync(bungalow);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteBungalowAsync(int id)
    {
        var bungalow = await _unitOfWork.GetRepository<Bungalow>().GetByIdAsync(id);
        if (bungalow != null)
        {
            _unitOfWork.GetRepository<Bungalow>().Delete(bungalow);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task<List<Bungalow>> GetAllBungalowsAsync()
    {
        return await _unitOfWork.GetRepository<Bungalow>().GetAllAsync(b => b.Category, b => b.Images);
    }

    public async Task<Bungalow?> GetBungalowByIdAsync(int id)
    {
        var result = await _unitOfWork.GetRepository<Bungalow>().GetByFilterAsync(b => b.Id == id, b => b.Category, b => b.Images);
        return result.FirstOrDefault();
    }

    public async Task<List<Bungalow>> GetBungalowsByFilterAsync(Expression<Func<Bungalow, bool>> filter)
    {
        return await _unitOfWork.GetRepository<Bungalow>().GetByFilterAsync(filter, b => b.Category, b => b.Images);
    }

    public async Task UpdateBungalowAsync(Bungalow bungalow)
    {
        _unitOfWork.GetRepository<Bungalow>().Update(bungalow);
        await _unitOfWork.SaveAsync();
    }
}
