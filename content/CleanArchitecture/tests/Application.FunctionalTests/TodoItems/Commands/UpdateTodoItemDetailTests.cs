using Cubido.Template.Application.TodoItems.Commands.CreateTodoItem;
using Cubido.Template.Application.TodoItems.Commands.UpdateTodoItem;
using Cubido.Template.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Cubido.Template.Domain.Entities;
using Cubido.Template.Domain.Enums;

namespace Cubido.Template.Application.FunctionalTests.TodoItems.Commands;

using static Testing;

public class UpdateTodoItemDetailTests : BaseTestFixture
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

        var command = new UpdateTodoItemDetailCommand
        {
            Id = itemId,
            ListId = list.Id,
            Note = "This is the note.",
            Priority = PriorityLevel.High
        };

        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.ShouldNotBeNull();
        item!.ListId.ShouldBe(command.ListId);
        item.Note.ShouldBe(command.Note);
        item.Priority.ShouldBe(command.Priority);
        item.LastModifiedBy.ShouldNotBeNull();
        item.LastModifiedBy.ShouldBe(userId);
        item.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
