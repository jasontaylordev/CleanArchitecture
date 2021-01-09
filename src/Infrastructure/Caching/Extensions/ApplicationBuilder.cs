using EasyCache.MemCache.Extensions;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Caching.Extensions
{
    /// <summary>
    /// This class includes extension method for EasyMemCache.
    /// </summary>
    public static class ApplicationBuilder
    {
        /// <summary>
        /// Use EasyMemCache
        /// </summary>
        /// <param name="app">
        /// Microsoft.AspNetCore.Builder.IApplicationBuilder
        /// </param>
        /// <returns>
        /// Microsoft.AspNetCore.Builder.IApplicationBuilder
        /// </returns>
        public static IApplicationBuilder UseEasyMemCache(this IApplicationBuilder app)
        {
            app.ApplyEasyMemCache();
            return app;
        }
    }
}
