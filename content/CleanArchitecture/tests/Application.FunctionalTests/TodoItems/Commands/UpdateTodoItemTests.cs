using Cubido.Template.Application.TodoItems.Commands.CreateTodoItem;
using Cubido.Template.Application.TodoItems.Commands.UpdateTodoItem;
using Cubido.Template.Domain.Entities;

namespace Cubido.Template.Application.FunctionalTests.TodoItems.Commands;

using static Testing;

public class UpdateTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemCommand { Id = 99, Title = "New Title" };

        await Should.ThrowAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldUpdateTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        var list = new TodoList() { Title = "New List" };
        await AddAsync(list);

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = list.Id,
            Title = "New Item"
        });

        var command = new UpdateTodoItemCommand
        {
            Id = itemId,
            Title = "Updated Item Title"
        };

        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.ShouldNotBeNull();
        item!.Title.ShouldBe(command.Title);
        item.LastModifiedBy.ShouldNotBeNull();
        item.LastModifiedBy.ShouldBe(userId);
        item.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
