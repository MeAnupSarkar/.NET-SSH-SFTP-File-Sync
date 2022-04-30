
using System.Linq.Expressions;

namespace MediaFon.FileManager.Core.Interfaces;

public interface IGenericRepository<T>   where T : class
{
    void Add(T entity);
    Task AddAsync(T entity);
    void AddRange(IEnumerable<T> entities);
    Task AddRangeAsync(IEnumerable<T> entities);
    IEnumerable<T> Find(Expression<Func<T, bool>> expression);
    IEnumerable<T> GetAll();
    T? GetById(Guid id);
    Task<T?> GetByIdAsync(Guid id);
    Task Update(T entity);
    Task UpdateRange(IEnumerable<T> entities);
    Task Remove(T entity);
    Task RemoveRange(IEnumerable<T> entities);



}
