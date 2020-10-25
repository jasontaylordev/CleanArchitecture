using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Attributes;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class CacheInvalidatorPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly ILogger _logger;
        private readonly InvalidateCacheForQueries _queriesPairs;
        public ICacheService CacheService { get; }

        public CacheInvalidatorPostProcessor(
            ILogger<CacheInvalidatorPostProcessor<TRequest, TResponse>> logger,
            InvalidateCacheForQueries cache,
            ICacheService cacheService)
        {
            _logger = logger;
            _queriesPairs = cache;
            CacheService = cacheService;
        }

        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            if (_queriesPairs.Count > 0)
            {
                foreach (var item in _queriesPairs)
                {
                    _logger.LogInformation($"For command {typeof(TRequest).Name}, the cache has been cleared for {item.Value.GetType().Name} query");

                    var cacheKey = CacheHelper.GenerateCacheKeyFromRequest(item.Value);
                    CacheService.RemoveCacheValue(cacheKey);
                }

                _queriesPairs.Clear();
            }

            return Task.CompletedTask;
        }
    }

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
            var cacheQuery = typeof(TRequest).GetCustomAttribute<CacheQueryResponseAttribute>();

            if (cacheQuery != null)
            {
                var cacheKey = string.IsNullOrEmpty(cacheQuery.CacheKey)
                    ? CacheHelper.GenerateCacheKeyFromRequest(request)
                    : cacheQuery.CacheKey;

                var cachedResponse = await CacheService.GetCacheValueAsync(cacheKey);
                if (cachedResponse != null)
                {
                    _logger.LogInformation($"Request {typeof(TRequest).Name} served from cache");
                    var data = (TResponse)cachedResponse;
                    return data;
                }

                var actualResponse = await next();
                await CacheService.SetCacheValueAsync(cacheKey, actualResponse, cacheQuery.TimeSpanForCacheInvalidation);
                return actualResponse;
            }
            return await next();
        }
    }

    public static class CacheHelper
    {
        public static string GenerateCacheKeyFromRequest(object request)
        {
            var key = new StringBuilder();
            key.Append($"{request.GetType().Name}|");
            foreach (var property in request.GetType().GetProperties())
            {
                key.Append($"{property.Name}|{property.GetValue(request)}|");
            }
            return key.ToString();
        }
    }
}
