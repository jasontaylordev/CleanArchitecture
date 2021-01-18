using CleanArchitecture.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Extensions
{
    public static class ApplicationCacheServiceExtension
    {
        public static T GetAndSet<T>(this IApplicationCacheService applicationCacheService, string key, Func<T> getResult, TimeSpan expireTime)
        {
            var data = applicationCacheService.Get<T>(key);

            if (data == null)
            {
                data = getResult();
                applicationCacheService.Set(key, data, expireTime);
            }

            return data;
        }

        public static async Task<T> GetAndSetAsync<T>(this IApplicationCacheService applicationCacheService, string key, Func<Task<T>> getResult, TimeSpan expireTime)
        {
            var data = applicationCacheService.Get<T>(key);

            if (data == null)
            {
                data = await getResult();
                applicationCacheService.Set(key, data, expireTime);
            }

            return data;
        }
    }
}
