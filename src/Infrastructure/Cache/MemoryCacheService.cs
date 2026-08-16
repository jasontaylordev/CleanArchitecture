using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Infrastructure.Cache;

public sealed class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> createItem, TimeSpan? absoluteExpirationRelativeToNow = null)
    {
        if (_memoryCache.TryGetValue(key, out TItem cachedItem))
        {
            return Task.FromResult(cachedItem);
        }

        return CreateAndCacheAsync(key, createItem, absoluteExpirationRelativeToNow);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    private async Task<TItem> CreateAndCacheAsync<TItem>(string key, Func<Task<TItem>> createItem, TimeSpan? absoluteExpirationRelativeToNow)
    {
        var item = await createItem();

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(1)
        };

        _memoryCache.Set(key, item, cacheEntryOptions);

        return item;
    }
}
