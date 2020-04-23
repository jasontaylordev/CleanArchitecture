using System;
using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface ICache<TRequest>
    {
        public CacheOptions SetCacheOptions() => new CacheOptions
        {
            CacheKey = $"{typeof(TRequest).FullName}",
            ExpirationRelativeToNow = TimeSpan.FromMilliseconds(60000)
        };
    }
}