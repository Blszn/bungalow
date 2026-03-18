using Bungalov.Core.Interfaces;
using Bungalov.DataAccess.Baglam;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bungalov.DataAccess.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
    public void Delete(T entity) => _context.Set<T>().Remove(entity);
    
    public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.ToListAsync();
    }

    public async Task<List<T>> GetByFilterAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>().Where(filter);
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);
    public void Update(T entity) => _context.Set<T>().Update(entity);
}