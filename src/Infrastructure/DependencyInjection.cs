using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Caching;
using CleanArchitecture.Infrastructure.Caching.Utilities;
using CleanArchitecture.Infrastructure.Files;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Services;
using EasyCache.MemCache.Concrete;
using EasyCache.MemCache.Extensions;
using EasyCache.Memory.Concrete;
using EasyCache.Memory.Extensions;
using EasyCache.Redis.Concrete;
using EasyCache.Redis.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("CleanArchitectureDb"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            }

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            services.AddScoped<IDomainEventService, DomainEventService>();

            services
                .AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator"));
            });

            if (configuration.GetValue<bool>(Constants.CACHE_OPTIONS_IS_ACTIVE))
            {
                switch (configuration.GetValue<string>(Constants.CACHE_OPTIONS_PROVIDER_NAME))
                {
                    case Constants.CACHE_REDIS:
                        RedisCacheOption redisCacheOption = new RedisCacheOption();
                        configuration.GetSection(Constants.CACHE_OPTIONS_REDIS).Bind(redisCacheOption, c => c.BindNonPublicProperties = true);
                        services.AddEasyRedisCache(options =>
                        {
                            options.Configuration = redisCacheOption.Configuration;
                            options.InstanceName = redisCacheOption.InstanceName;
                        });
                        services.AddTransient<IApplicationCacheService, ApplicationEasyRedisCacheManager>();
                        break;
                    case Constants.CACHE_MEMORY:
                        services.AddEasyMemoryCache();
                        services.AddTransient<IApplicationCacheService, ApplicationEasyMemoryCacheManager>();
                        break;
                    case Constants.CACHE_MEMCACHE:
                        MemCacheOption memCacheOption = new MemCacheOption();
                        configuration.GetSection(Constants.CACHE_OPTIONS_MEMCACHE).Bind(memCacheOption, c => c.BindNonPublicProperties = true);
                        services.AddEasyMemCache(options => options.AddServer(memCacheOption.ServerName, memCacheOption.Port));
                        services.AddTransient<IApplicationCacheService, ApplicationEasyMemCacheManager>();
                        break;
                    default:
                        break;
                }
            }

            return services;
        }
    }
}