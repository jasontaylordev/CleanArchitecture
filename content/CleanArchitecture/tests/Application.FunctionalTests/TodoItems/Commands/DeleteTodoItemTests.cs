using Cubido.Template.Application.TodoItems.Commands.CreateTodoItem;
using Cubido.Template.Application.TodoItems.Commands.DeleteTodoItem;
using Cubido.Template.Domain.Entities;

namespace Cubido.Template.Application.FunctionalTests.TodoItems.Commands;

using static Testing;

public class DeleteTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new DeleteTodoItemCommand() { Id = 99 };

        await Should.ThrowAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldDeleteTodoItem()
    {
        var list = new TodoList() { Title = "New List" };
        await AddAsync(list);

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = list.Id,
            Title = "New Item"
        });

        await SendAsync(new DeleteTodoItemCommand() { Id = itemId });

        var item = await FindAsync<TodoItem>(itemId);

        item.ShouldBeNull();
    }
}
