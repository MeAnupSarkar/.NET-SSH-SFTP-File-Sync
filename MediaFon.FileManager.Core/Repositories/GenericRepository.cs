using MediaFon.FileManager.Core.Interfaces;
using MediaFon.FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MediaFon.FileManager.Core.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext context;

    public GenericRepository(ApplicationDbContext _context) => context = _context;

    public T? GetById(Guid id) => context.Set<T>().Find(id);

    public bool Any(Expression<Func<T, bool>> expression) => context.Set<T>().Any(expression);

    public async Task<T?> GetByIdAsync(Guid id) => await context.Set<T>().FindAsync(id);

    public IEnumerable<T> Find(Expression<Func<T, bool>> expression) => context.Set<T>().Where(expression);

    public IEnumerable<T> GetAll() => context.Set<T>().ToList();

    public async Task<IEnumerable<T>> GetAllAsync() => await context.Set<T>().ToListAsync();

    public void Add(T entity) => context.Set<T>().Add(entity);

    public async Task AddAsync(T entity) => await context.Set<T>().AddAsync(entity);

    public void AddRange(IEnumerable<T> entities) => context.Set<T>().AddRange(entities);

    public async Task AddRangeAsync(IEnumerable<T> entities) => await context.Set<T>().AddRangeAsync(entities);

    public async Task Update(T entity) => context.Set<T>().Update(entity);
 
    public async Task UpdateRange(IEnumerable<T> entities) =>  await context.SaveChangesAsync();
    
    public async Task Remove(T entity) => context.Set<T>().Remove(entity);
       
    public async Task RemoveRange(IEnumerable<T> entities) => context.Set<T>().RemoveRange(entities);
   
}
