using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace CleanArchitecture.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<object> GetCacheValueAsync(string key)
        {
            var cached = await _distributedCache.GetStringAsync(key);
            if (cached != null)
                return await Task.FromResult(JsonConvert.DeserializeObject(cached, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects }));

            return await Task.FromResult<object>(null);
        }

        public async Task RemoveCacheValue(string key)
        {
            if (key != null)
            {
                await _distributedCache.RemoveAsync(key);
            }
        }

        public async Task SetCacheValueAsync(string key, object value, TimeSpan expirationTimeFromNow)
        {
            var serializedResponse = JsonConvert.SerializeObject(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });

            await _distributedCache.SetStringAsync(key, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTimeFromNow
            });
        }
    }
}
