using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Caching.Utilities
{
    /// <summary>
    /// This static class includes magic strings for Easy Cache
    /// </summary>
    internal static class Constants
    {
        public static string CACHE_OPTIONS_IS_ACTIVE = "CacheOptions:IsActive";
        public static string CACHE_OPTIONS_PROVIDER_NAME = "CacheOptions:ProviderName";
        public const string CACHE_REDIS = "Redis";
        public const string CACHE_MEMCACHE = "MemCache";
        public const string CACHE_MEMORY = "Memory";
        public static string CACHE_OPTIONS_REDIS = "CacheOptions:RedisCache";
        public static string CACHE_OPTIONS_MEMCACHE = "CacheOptions:MemCache";
    }
}
