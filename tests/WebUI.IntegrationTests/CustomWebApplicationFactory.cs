using CleanArchitecture.Application;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence;
using IdentityModel.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CleanArchitecture.WebUI.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
                {
                    // Remove the app's ApplicationDbContext registration.
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                            typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add a database context using an in-memory 
                    // database for testing.
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    // Register test services
                    services.AddScoped<ICurrentUserService, TestCurrentUserService>();
                    services.AddScoped<IDateTime, TestDateTimeService>();
                    services.AddScoped<IIdentityService, TestIdentityService>();

                    // Build the service provider
                    var sp = services.BuildServiceProvider();

                    // Create a scope to obtain a reference to the database
                    // context (ApplicationDbContext).
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                        var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                        // Ensure the database is created.
                        context.Database.EnsureCreated();

                        try
                        {
                            // Seed the database with test data.
                            SeedSampleData(context);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {Message}", ex.Message);
                        }
                    }
                })
                .UseEnvironment("Test");
        }

        public HttpClient GetAnonymousClient()
        {
            return CreateClient();
        }

        public async Task<HttpClient> GetAuthenticatedClientAsync()
        {
            return await GetAuthenticatedClientAsync("jason@clean-architecture", "CleanArchitecture!");
        }

        public async Task<HttpClient> GetAuthenticatedClientAsync(string userName, string password)
        {
            var client = CreateClient();

            var token = await GetAccessTokenAsync(client, userName, password);

            client.SetBearerToken(token);

            return client;
        }

        private async Task<string> GetAccessTokenAsync(HttpClient client, string userName, string password)
        {
            var disco = await client.GetDiscoveryDocumentAsync();

            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "CleanArchitecture.IntegrationTests",
                ClientSecret = "secret",

                Scope = "CleanArchitecture.WebUIAPI openid profile",
                UserName = userName,
                Password = password
            });

            if (response.IsError)
            {
                throw new Exception(response.Error);
            }

            return response.AccessToken;
        }

        public static void SeedSampleData(ApplicationDbContext context)
        {
            context.TodoItems.AddRange(
                new TodoItem { Id = 1, Title = "Do this thing." },
                new TodoItem { Id = 2, Title = "Do this thing too." },
                new TodoItem { Id = 3, Title = "Do many, many things." },
                new TodoItem { Id = 4, Title = "This thing is done!", Done = true }
            );

            context.SaveChanges();
        }
    }
}
