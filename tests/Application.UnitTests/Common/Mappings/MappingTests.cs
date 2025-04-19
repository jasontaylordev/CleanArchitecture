using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using CleanArchitecture.Domain.Entities;
using Shouldly;
using NUnit.Framework;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    [Test]
    public void Should_Map_TodoList_To_TodoListDto()
    {
        var list = new TodoList
        {
            Id = 1,
            Title = "Groceries",
            Colour = Colour.Red
        };

        list.Items.Add(new TodoItem
        {
            Id = 10,
            ListId = 1,
            Title = "Buy milk",
            Done = true,
            Priority = PriorityLevel.High,
            Note = "2 liters"
        });

        var dto = TodoListDtoMapper.FromEntity(list);

        dto.Id.ShouldBe(list.Id);
        dto.Title.ShouldBe(list.Title);
        dto.Colour.ShouldBe(list.Colour.ToString());
        dto.Items.Count.ShouldBe(1);
        dto.Items.First().Title.ShouldBe("Buy milk");
    }

    [Test]
    public void Should_Map_TodoItem_To_TodoItemDto()
    {
        var item = new TodoItem
        {
            Id = 5,
            ListId = 2,
            Title = "Walk the dog",
            Done = false,
            Priority = PriorityLevel.Medium,
            Note = "Evening"
        };

        var dto = TodoItemDtoMapper.FromEntity(item);

        dto.Id.ShouldBe(item.Id);
        dto.ListId.ShouldBe(item.ListId);
        dto.Title.ShouldBe(item.Title);
        dto.Done.ShouldBe(item.Done);
        dto.Priority.ShouldBe((int)item.Priority);
        dto.Note.ShouldBe(item.Note);
    }

    [Test]
    public void Should_Map_TodoList_To_LookupDto()
    {
        var list = new TodoList { Id = 99, Title = "Chores" };

        var dto = LookupDtoMapper.FromTodoList(list);

        dto.Id.ShouldBe(list.Id);
        dto.Title.ShouldBe(list.Title);
    }

    [Test]
    public void Should_Map_TodoItem_To_LookupDto()
    {
        var item = new TodoItem { Id = 42, Title = "Do laundry" };

        var dto = LookupDtoMapper.FromTodoItem(item);

        dto.Id.ShouldBe(item.Id);
        dto.Title.ShouldBe(item.Title);
    }

    [Test]
    public void Should_Map_TodoItem_To_BriefDto()
    {
        var item = new TodoItem
        {
            Id = 7,
            ListId = 3,
            Title = "Read book",
            Done = true
        };

        var dto = TodoItemBriefDtoMapper.FromEntity(item);

        dto.Id.ShouldBe(item.Id);
        dto.ListId.ShouldBe(item.ListId);
        dto.Title.ShouldBe(item.Title);
        dto.Done.ShouldBe(item.Done);
    }
}
