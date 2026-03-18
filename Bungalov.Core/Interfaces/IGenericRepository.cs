using System.Linq.Expressions;

namespace Bungalov.Core.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetByFilterAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}