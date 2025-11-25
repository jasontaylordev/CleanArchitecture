using Cubido.Template.Application.Common.Interfaces;
using Cubido.Template.Application.FunctionalTests.TestServices;
using Cubido.Template.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;

namespace Cubido.Template.Application.FunctionalTests;

using static Testing;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection connection;

    public CustomWebApplicationFactory(DbConnection connection)
    {
        this.connection = connection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IUser>()
                .AddTransient(provider => GetIUserMock());
            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                    options.UseSqlServer(connection);
                });

            services
                .RemoveAll<TimeProvider>()
                .AddSingleton<TimeProvider>(new TestTimeProvider());
        });
    }

    private static IUser GetIUserMock()
    {
        var userSubstitute = Substitute.For<IUser>();
        userSubstitute.Id.Returns(GetUserId());
        return userSubstitute;
    }
}
