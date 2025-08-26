using LuckShot.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LuckShot.Infrastructure.Repositories;

public interface IRepository<T, in TKey> where T : class
{
    Task<T?> GetByIdAsync(TKey id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
    Task<bool> ExistsAsync(TKey id);
}


public abstract class Repository<T, TKey>(LuckyShotContext context) : IRepository<T, TKey> where T : class
{
    private DbSet<T> DbSet => context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(TKey id) => await DbSet.FindAsync(id);

    public virtual async Task AddAsync(T entity)
    {
        try
        {
            DbSet.Add(entity);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new RepositoryException("An unexpected error occurred while adding the entity.", e);
        }
    }

    public virtual async Task UpdateAsync(T entity)
    {
        try
        {
            DbSet.Update(entity);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException e)
        {
            throw new RepositoryConcurrencyException("Concurrency conflict occurred while updating the entity.", e);
        }
        catch (Exception e)
        {
            throw new RepositoryException("An unexpected error occurred while updating the entity.", e);
        }
    }

    public virtual async Task RemoveAsync(T entity)
    {
        try
        {
            DbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException e)
        {
            throw new RepositoryConcurrencyException("Concurrency conflict occurred while removing the entity.", e);
        }
        catch (Exception e)
        {
            throw new RepositoryException("An unexpected error occurred while removing the entity.", e);
        }
    }

    public abstract Task<bool> ExistsAsync(TKey id);
}