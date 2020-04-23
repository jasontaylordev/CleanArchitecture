using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;
        private ICacheService CacheService { get; }

        public CachingBehaviour(ICacheService cacheService, ILogger<CachingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
            CacheService = cacheService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var needCaching = typeof(ICache<TRequest>).IsAssignableFrom(typeof(TRequest));
            if (needCaching)
            {
                string cacheKey;
                var options = new CacheOptions();

                var methodReference = typeof(TRequest).GetMethod("SetCacheOptions");
                if (methodReference != null)
                {
                    options = (CacheOptions)methodReference.Invoke(request, new object[] { });
                    cacheKey = options.CacheKey;
                }
                else
                {
                    cacheKey = GenerateCacheKeyFromRequest(request);
                }

                var cachedResponse = await CacheService.GetCacheValueAsync(cacheKey);
                if (cachedResponse != null)
                {
                    _logger.LogInformation($"Request {typeof(TRequest).Name} served from cache");
                    var data = (TResponse)cachedResponse;
                    return data;
                }

                var actualResponse = await next();
                await CacheService.SetCacheValueAsync(cacheKey, actualResponse, options.ExpirationRelativeToNow == TimeSpan.Zero ? TimeSpan.FromMilliseconds(60000) : options.ExpirationRelativeToNow);
                return actualResponse;
            }

            return await next();
        }

        private static string GenerateCacheKeyFromRequest(TRequest request)
        {
            var key = new StringBuilder();

            key.Append($"{typeof(TRequest).Name}|");

            foreach (var property in request.GetType().GetProperties())
            {
                key.Append($"{property.Name}| {property.GetValue(request)}|");
            }
            return key.ToString();
        }
    }
}
