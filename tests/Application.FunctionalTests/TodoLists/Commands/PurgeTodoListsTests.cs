﻿using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.PurgeTodoLists;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.FunctionalTests.TodoLists.Commands;

using static Testing;

public class PurgeTodoListsTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var command = new PurgeTodoListsCommand();

        command.GetType().ShouldSatisfyAllConditions(
            type => type.ShouldBeDecoratedWith<AuthorizeAttribute>()
        );

        var action = () => SendAsync(command);

        await Should.ThrowAsync<UnauthorizedAccessException>(action);
    }

    [Test]
    public async Task ShouldDenyNonAdministrator()
    {
        await RunAsDefaultUserAsync();

        var command = new PurgeTodoListsCommand();

        var action = () => SendAsync(command);

        await Should.ThrowAsync<ForbiddenAccessException>(action);
    }

    [Test]
    public async Task ShouldAllowAdministrator()
    {
        await RunAsAdministratorAsync();

        var command = new PurgeTodoListsCommand();

        var action = () => SendAsync(command);

        Func<Task> asyncAction = async () => await SendAsync(command);
        await asyncAction.ShouldNotThrowAsync();
    }

    [Test]
    public async Task ShouldDeleteAllLists()
    {
        await RunAsAdministratorAsync();

        await SendAsync(new CreateTodoListCommand
        {
            Title = "New List #1"
        });

        await SendAsync(new CreateTodoListCommand
        {
            Title = "New List #2"
        });

        await SendAsync(new CreateTodoListCommand
        {
            Title = "New List #3"
        });

        await SendAsync(new PurgeTodoListsCommand());

        var count = await CountAsync<TodoList>();

        count.ShouldBe(0);
    }
}
