using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.FunctionalTests.TodoLists.Queries;

public class GetTodosTests : TestBase
{
    [Test]
    public async Task ShouldReturnPriorityLevels()
    {
        await TestApp.RunAsDefaultUserAsync();

        var query = new GetTodosQuery();

        var result = await TestApp.SendAsync(query);

        result.PriorityLevels.ShouldNotBeEmpty();
    }

    [Test]
    public async Task ShouldReturnAllListsAndItems()
    {
        await TestApp.RunAsDefaultUserAsync();

        await TestApp.AddAsync(new TodoList
        {
            Title = "Shopping",
            Colour = Colour.Blue,
            Items =
                {
                    new TodoItem { Title = "Apples", Done = true },
                    new TodoItem { Title = "Milk", Done = true },
                    new TodoItem { Title = "Bread", Done = true },
                    new TodoItem { Title = "Toilet paper" },
                    new TodoItem { Title = "Pasta" },
                    new TodoItem { Title = "Tissues" },
                    new TodoItem { Title = "Tuna" }
                }
        });

        var query = new GetTodosQuery();

        var result = await TestApp.SendAsync(query);

        result.Lists.Count.ShouldBe(1);
        result.Lists.First().Items.Count.ShouldBe(7);
    }

    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new GetTodosQuery();

        var action = () => TestApp.SendAsync(query);

        await Should.ThrowAsync<UnauthorizedAccessException>(action);
    }
}
