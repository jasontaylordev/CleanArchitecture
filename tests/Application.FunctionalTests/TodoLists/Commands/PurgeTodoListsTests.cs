using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.PurgeTodoLists;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.FunctionalTests.TodoLists.Commands;

public class PurgeTodoListsTests : TestBase
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var command = new PurgeTodoListsCommand();

        command.GetType().ShouldSatisfyAllConditions(
            type => type.ShouldBeDecoratedWith<AuthorizeAttribute>()
        );

        var action = () => TestApp.SendAsync(command);

        await Should.ThrowAsync<UnauthorizedAccessException>(action);
    }

    [Test]
    public async Task ShouldDenyNonAdministrator()
    {
        await TestApp.RunAsDefaultUserAsync();

        var command = new PurgeTodoListsCommand();

        var action = () => TestApp.SendAsync(command);

        await Should.ThrowAsync<ForbiddenAccessException>(action);
    }

    [Test]
    public async Task ShouldAllowAdministrator()
    {
        await TestApp.RunAsAdministratorAsync();

        var command = new PurgeTodoListsCommand();

        var action = () => TestApp.SendAsync(command);

        Func<Task> asyncAction = async () => await TestApp.SendAsync(command);
        await asyncAction.ShouldNotThrowAsync();
    }

    [Test]
    public async Task ShouldDeleteAllLists()
    {
        await TestApp.RunAsAdministratorAsync();

        await TestApp.SendAsync(new CreateTodoListCommand
        {
            Title = "New List #1"
        });

        await TestApp.SendAsync(new CreateTodoListCommand
        {
            Title = "New List #2"
        });

        await TestApp.SendAsync(new CreateTodoListCommand
        {
            Title = "New List #3"
        });

        await TestApp.SendAsync(new PurgeTodoListsCommand());

        var count = await TestApp.CountAsync<TodoList>();

        count.ShouldBe(0);
    }
}
