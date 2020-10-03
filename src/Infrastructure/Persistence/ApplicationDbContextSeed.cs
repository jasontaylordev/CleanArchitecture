using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
            {
                await userManager.CreateAsync(defaultUser, "Administrator1!");
            }
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            // Seed, if necessary
            if (!context.TodoLists.Any())
            {
                await context.TodoLists.AddAsync(new TodoList("Shopping") { Items =
                {
                    new TodoItem(1, "Apples", true),
                    new TodoItem(1, "Milk", true),
                    new TodoItem(1, "Bread", true),
                    new TodoItem(1, "Toilet paper", true),
                    new TodoItem(1, "Pasta"),
                    new TodoItem(1, "Tissues"),
                    new TodoItem(1, "Tuna"),
                    new TodoItem(1, "Water"),

                }});

                await context.SaveChangesAsync();
            }
        }
    }
}
