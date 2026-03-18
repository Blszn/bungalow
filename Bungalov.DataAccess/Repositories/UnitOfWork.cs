using Bungalov.Core.Interfaces;
using Bungalov.DataAccess.Baglam;

namespace Bungalov.DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        return new GenericRepository<T>(_context);
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
