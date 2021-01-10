using CleanArchitecture.Application.Common.Interfaces;
using EasyCache.Redis.Concrete;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Caching
{
    public class ApplicationEasyRedisCacheManager : EasyCacheRedisManager, IApplicationCacheService
    {
        public ApplicationEasyRedisCacheManager(IDistributedCache distributedCache) : base(distributedCache)
        {
        }
    }
}
