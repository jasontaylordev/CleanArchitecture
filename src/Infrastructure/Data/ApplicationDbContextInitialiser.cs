using System.Reflection;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static void AddAsyncSeeding(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider)
    {
        builder.UseAsyncSeeding(async (context, _, ct) =>
        {
            var initialiser = serviceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

            await initialiser.SeedAsync();
        });
    }

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync(app.Environment.IsDevelopment());
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<User> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync(bool isDevelopment)
    {
        try
        {
            // Can add more condition, _context.Database.IsNpgsql or _context.Database.IsSqlServer based on the default database.
            if (!isDevelopment)
            {
                await _context.Database.MigrateAsync();
            }
            else if (isDevelopment)
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.EnsureCreatedAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminAsync();

        // Default data
        // Seed, if necessary
        if (!_context.TodoLists.Any())
        {
            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List",
                Items =
                {
                    new TodoItem { Title = "Make a todo list 📃" },
                    new TodoItem { Title = "Check off the first item ✅" },
                    new TodoItem { Title = "Realise you've already done two things on the list! 🤯"},
                    new TodoItem { Title = "Reward yourself with a nice, long nap 🏆" },
                }
            });

            await _context.SaveChangesAsync();
        }
    }

    // =============Helpers=============
    async Task SeedRolesAsync()
    {
        try
        {
            // You can add N of Roles inside this Roles Constant.
            var roles = typeof(Roles)
                .GetFields(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Static)
                .Where(t => t.IsLiteral && !t.IsInitOnly)
                .Select(property => property.GetRawConstantValue()?.ToString())
                .Select(role => new ApplicationRole(role!))
                .ToList();

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name!))
                {
                    await _roleManager.CreateAsync(role);
                    _logger.LogInformation("Role {roleName} added", role.Name);
                }
                else
                {
                    _logger.LogInformation("Skipping {roleName}, it is already added.", role.Name);
                }
            }
            _logger.LogInformation("Roles {roles} are successfully added.", string.Join(';', roles.Select(r => r.Name)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured during seeding roles.");
            throw;
        }
    }

    async Task SeedAdminAsync()
    {
        try
        {
            var user = new User("Admin user", "administrator@localhost");

            if (_userManager.Users.All(x => x.UserName != user.UserName))
            {
                await _userManager.CreateAsync(user, "Administrator1!");
                await _userManager.AddToRoleAsync(user, Roles.Administrator);

                _logger.LogInformation("Admin user added.");
            }
            else
            {
                _logger.LogInformation("Skipping {displayName}, {username} already existed.", user.DisplayName, user.Email);
            }

            _logger.LogInformation("Seeding admin user completed.");
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Failed to seeding admin user.");
            throw;
        }
    }
}
