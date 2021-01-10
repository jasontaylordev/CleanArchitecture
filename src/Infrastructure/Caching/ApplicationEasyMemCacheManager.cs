using CleanArchitecture.Application.Common.Interfaces;
using EasyCache.MemCache.Concrete;
using Enyim.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Caching
{
    public class ApplicationEasyMemCacheManager : EasyCacheMemCacheManager, IApplicationCacheService
    {
        public ApplicationEasyMemCacheManager(IMemcachedClient memcachedClient) : base(memcachedClient)
        {
        }
    }
}
