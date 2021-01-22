using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Attributes;
using CleanArchitecture.Application.Common.Interfaces;
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
			var cacheQuery = typeof(TRequest).GetCustomAttribute<CacheQueryResponseAttribute>();
			var commandRequest = typeof(TRequest).GetCustomAttribute<InvalidateCacheAttribute>();

			if (cacheQuery != null)
			{
				return await HandleQuery(cacheQuery, request, next);
			}

			if (commandRequest != null && commandRequest.Queries != Type.EmptyTypes)
			{
				await HandleCommand(commandRequest);
			}

			return await next();
		}

		private async Task<TResponse> HandleQuery(CacheQueryResponseAttribute cacheQuery, TRequest request, RequestHandlerDelegate<TResponse> next)
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

		private async Task HandleCommand(InvalidateCacheAttribute commandRequest)
		{
			foreach (var type in commandRequest.Queries)
			{
				var queryType = type.GetCustomAttribute<CacheQueryResponseAttribute>();
				var key = string.IsNullOrEmpty(queryType.CacheKey)
					? CacheHelper.GenerateCacheKeyFromRequest(Activator.CreateInstance(type))
					: queryType.CacheKey;

				await CacheService.RemoveCacheValue(key);
			}
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
