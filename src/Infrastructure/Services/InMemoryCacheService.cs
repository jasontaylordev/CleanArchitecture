using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Infrastructure.Services
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public InMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<object> GetCacheValueAsync(string key)
        {
            if (_memoryCache.TryGetValue<object>(key, out var cacheResponse))
            {
                return Task.FromResult(cacheResponse);
            }
            return Task.FromResult<object>(null);
        }

        public Task SetCacheValueAsync(string key, object value, TimeSpan expirationTimeFromNow)
        {
            _memoryCache.Set(key, value, absoluteExpirationRelativeToNow: expirationTimeFromNow);
            return Task.CompletedTask;
        }

        public Task RemoveCacheValue(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
