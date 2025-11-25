using Cubido.Template.Application.Common.Exceptions;
using Cubido.Template.Application.TodoItems.Commands.CreateTodoItem;
using Cubido.Template.Domain.Entities;

namespace Cubido.Template.Application.FunctionalTests.TodoItems.Commands;

using static Testing;

public class CreateTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTodoItemCommand();

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        var list = new TodoList() { Title = "New List" };
        await AddAsync(list);

        var command = new CreateTodoItemCommand
        {
            ListId = list.Id,
            Title = "Tasks"
        };

        var itemId = await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.ShouldNotBeNull();
        item!.ListId.ShouldBe(command.ListId);
        item.Title.ShouldBe(command.Title);
        item.CreatedBy.ShouldBe(userId);
        item.Created.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        item.LastModifiedBy.ShouldBe(userId);
        item.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
