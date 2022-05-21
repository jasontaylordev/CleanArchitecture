using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CleanArchitecture.Application.IntegrationTests;

public partial class Testing
{
    private class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices((builder, services) =>
            {
                services
                    .Remove<ICurrentUserService>()
                    .AddTransient(provider => Mock.Of<ICurrentUserService>(s => s.UserId == _currentUserId));

                services
                    .Remove<DbContextOptions<ApplicationDbContext>>()
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(
                            builder.Configuration.GetConnectionString("DefaultConnection"),
                            b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            });

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "UseInMemoryDatabase", "false" },
                    { "ConnectionStrings:DefaultConnection", "Server=(localdb)\\mssqllocaldb;Database=CleanArchitectureTestDb;Trusted_Connection=True;MultipleActiveResultSets=true;" },
                });
            });
       }
    }
}
