using Cubido.Template.Application.Queries.GetTodoItemsWithPagination;
using Cubido.Template.Domain.Entities;
using Shouldly;

namespace Cubido.Template.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    [Test]
    public void ShouldMapTodoItem()
    {
        var item = new TodoItem()
        {
            Id = 1,
            ListId = 3,
            Title = "Test Todo Item",
            Done = false
        };

        var dto = item.Map();

        dto.Id.ShouldBe(item.Id);
        dto.ListId.ShouldBe(item.ListId);
        dto.Title.ShouldBe(item.Title);
        dto.Done.ShouldBe(item.Done);
    }

    [Test]
    public void ShouldMapIQueryableTodoItem()
    {
        var item = new TodoItem()
        {
            Id = 1,
            ListId = 3,
            Title = "Test Todo Item",
            Done = false
        };

        var projection = new TodoItem[] { item }.AsQueryable().ProjectTo();
        var dto = projection.Single();

        dto.Id.ShouldBe(item.Id);
        dto.ListId.ShouldBe(item.ListId);
        dto.Title.ShouldBe(item.Title);
        dto.Done.ShouldBe(item.Done);
    }
}
