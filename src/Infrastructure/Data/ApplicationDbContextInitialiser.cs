using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
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
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }

        var assignee = new ApplicationUser { UserName = "assignee@localhost", Email = "assignee@localhost" };

        if (_userManager.Users.All(u => u.UserName != assignee.UserName))
        {
            await _userManager.CreateAsync(assignee, "Assignee1!");
        }

        if (!_context.TodoLists.Any())
        {
            _context.TodoLists.Add(new TodoList
            {
                Title = "Tasks",
                Colour = Colour.Green,
                Items =
                {
                    new TodoItem { Title = "Make a todo list 📃" },
                    new TodoItem { Title = "Check off the first item ✅" },
                    new TodoItem { Title = "Realise you've already done two things on the list! 🤯"},
                    new TodoItem { Title = "Reward yourself with a nice, long nap 🏆" },
                }
            });
        }

        if (!await _context.Projects.AnyAsync())
        {
            var project = new Project
            {
                Name = "Demo Project",
                Description = "Seed project for the take-home Project Management module."
            };

            var todo = new ProjectTodoStatus { Name = "To Do", SortOrder = 10, IsDefault = true };
            var progress = new ProjectTodoStatus { Name = "In Progress", SortOrder = 20 };
            var done = new ProjectTodoStatus { Name = "Done", SortOrder = 30, IsTerminal = true };

            project.Statuses.Add(todo);
            project.Statuses.Add(progress);
            project.Statuses.Add(done);

            project.TodoItems.Add(new ProjectTodoItem
            {
                Title = "Item: Review project management module",
                Description = "Use the list and Kanban views to verify the end-to-end workflow.",
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(2)),
                AssigneeUserId = assignee.Id,
                ReporterUserId = administrator.Id,
                Status = todo
            });

            _context.Projects.Add(project);
        }

        await _context.SaveChangesAsync();
    }
}
