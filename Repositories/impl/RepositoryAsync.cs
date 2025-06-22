using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Repositories.impl;

public class RepositoryAsync<T> : IRepositoryAsync<T> where T : class
{
    private readonly Sep490EduseenContext _context;
    private readonly DbSet<T> _dbSet;

    public RepositoryAsync(Sep490EduseenContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAsync(
        CancellationToken cancellationToken = default)
    {
       IQueryable<T> query = _dbSet;
       var data = query.ToListAsync(cancellationToken);
       return await data;
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if(entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken) 
               ?? throw new InvalidOperationException($"Entity with ID {id} not found.");
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<IEnumerable<T>> FindAsync(
    Expression<Func<T, bool>> predicate,
    CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }
}