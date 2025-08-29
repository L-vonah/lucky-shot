using LuckyShot.Domain.Entities;

namespace LuckyShot.Domain;

public interface IRepository<T, in TKey> where T : DatabaseEntity
{
    Task<T?> GetByIdAsync(TKey id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
    Task<bool> ExistsAsync(TKey id);
}