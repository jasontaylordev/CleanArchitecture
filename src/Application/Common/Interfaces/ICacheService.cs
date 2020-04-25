using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface ICacheService
    {
        public Task<object> GetCacheValueAsync(string key);

        public Task SetCacheValueAsync(string key, object value, TimeSpan expirationTimeFromNow);

        public Task RemoveCacheValue(string key);
    }
}