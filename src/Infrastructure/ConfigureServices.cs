using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Files;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Interceptors;
using CleanArchitecture.Infrastructure.Services;
using IBM.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("CleanArchitectureDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseDb2(configuration.GetConnectionString("DefaultConnection"),
                        p => { p.SetServerInfo(IBMDBServerType.AS400); p.UseRowNumberForPaging(); })
                        .UseLoggerFactory(
                            LoggerFactory.Create(builder =>
                            {
                                builder.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information)
                                       .AddConsole();
                            }
                        ))
                        .ReplaceService<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>());
        }

        services.AddScoped<IDbContextSchema, DbContextSchema>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // services.AddScoped<ApplicationDbContextInitialiser>();

        // services
        //     .AddDefaultIdentity<ApplicationUser>()
        //     .AddRoles<IdentityRole>()
        //     .AddEntityFrameworkStores<ApplicationDbContext>();

        // services.AddIdentityServer()
        //     .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddTransient<IDateTime, DateTimeService>();
        // services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

        // services.AddAuthorization(options =>
        //     options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator")));

        return services;
    }
}
