using CleanArchitecture.Application.Common.Interfaces;
using EasyCache.Memory.Concrete;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Caching
{
    public class ApplicationEasyMemoryCacheManager : EasyMemoryCacheManager, IApplicationCacheService
    {
        public ApplicationEasyMemoryCacheManager(IMemoryCache memoryCache) : base(memoryCache)
        {
        }
    }
}
