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
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync(app.Environment.IsDevelopment());
        await initialiser.SeedAsync();
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
            // You can add .IsNpgsql or .IsSqlServer or .IsSqlite method with isDevelopment, ex :  if (isDevelopment && _context.Database.IsSqlServer())
            if (isDevelopment)
            {
                // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.EnsureCreatedAsync();
            }
            else if (!isDevelopment)
            {
                await _context.Database.MigrateAsync();
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
        await SeedAdminUserAsync();

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

    //============== Helpers ==============
    async Task SeedRolesAsync()
    {
        try
        {
            var roles = typeof(Roles)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Static)
                .Where(t => t.IsLiteral && !t.IsInitOnly)
                .Select(st => st.GetRawConstantValue()?.ToString())
                .Select(role => new ApplicationRole(role!))
                .ToList();

            var tasks = roles.Select(async role =>
            {
                try
                {
                    if (!await _roleManager.RoleExistsAsync(role.Name!))
                    {
                        await _roleManager.CreateAsync(role);
                        _logger.LogInformation("{roleName} added", role.Name);
                    }
                    else
                    {
                        _logger.LogInformation("Skipping {roleName}, already exist.", role.Name ?? "<null>");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured while seeding {roleName}", role.Name);
                    throw;
                }
            });

            await Task.WhenAll(tasks);

            _logger.LogInformation("Roles {roles} are added successfully.", string.Join(';', roles
                .Select(r => r.Name)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while seeding roles");
            throw;
        }
    }

    async Task SeedAdminUserAsync()
    {
        try
        {
            var user = new User("Admin user", "administrator@localhost");

            if (_userManager.Users.All(x => x.UserName != user.UserName))
            {
                await _userManager.CreateAsync(user, "Administrator1!");
                await _userManager.AddToRoleAsync(user, Roles.Administrator);
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "An error occured while seeding admin user.");
            throw;
        }
    }
}
