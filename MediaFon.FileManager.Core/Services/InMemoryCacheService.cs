using MediaFon.FileManager.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
 
namespace MediaFon.FileManager.Core.Services;

// Initial plan was implementing In Memory Cache for better performance.
// But at last quitted the idea because I got very exhausted by doing continious coding.
public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    
    private MemoryCacheEntryOptions _cacheOptions;
    public InMemoryCacheService(IMemoryCache memoryCache )
    {
        _memoryCache = memoryCache;
        
        _cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddHours(6),
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromMinutes(60)
        };
       
    }
    public bool TryGet<T>(string cacheKey, out T value)
    {
        _memoryCache.TryGetValue(cacheKey, out value);
        if (value == null) return false;
        else return true;
    }
    public T Set<T>(string cacheKey, T value)
    {
        return _memoryCache.Set(cacheKey, value, _cacheOptions);
    }
    public void Remove(string cacheKey)
    {
        _memoryCache.Remove(cacheKey);
    }
}
