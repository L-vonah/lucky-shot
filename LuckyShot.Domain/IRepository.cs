using LuckyShot.Domain.Entities;

namespace LuckyShot.Domain;

public interface IRepository<T, in TKey> where T : DatabaseEntity
{
    Task<T?> GetByIdAsync(TKey id);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(IEnumerable<T> entities);
    Task RemoveAsync(T entity);
    Task<bool> ExistsAsync(TKey id);
}