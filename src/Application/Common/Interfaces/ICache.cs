using System;
using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface ICache
    {
        public CacheOptions SetCacheOptions() => new CacheOptions
        {
            CacheKey = string.Empty,
            ExpirationRelativeToNow = TimeSpan.FromMilliseconds(60000)
        };
    }
}