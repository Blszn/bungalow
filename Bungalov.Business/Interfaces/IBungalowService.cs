using Bungalov.Core.Varliklar;
using System.Linq.Expressions;

namespace Bungalov.Business.Interfaces;

public interface IBungalowService
{
    Task<List<Bungalow>> GetAllBungalowsAsync();
    Task<Bungalow?> GetBungalowByIdAsync(int id);
    Task<List<Bungalow>> GetBungalowsByFilterAsync(Expression<Func<Bungalow, bool>> filter);
    Task AddBungalowAsync(Bungalow bungalow);
    Task UpdateBungalowAsync(Bungalow bungalow);
    Task DeleteBungalowAsync(int id);
}
