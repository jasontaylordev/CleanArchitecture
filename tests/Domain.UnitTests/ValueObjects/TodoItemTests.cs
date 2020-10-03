using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Domain.UnitTests.ValueObjects
{
    public class TodoItemTests
    {
        [Test]
        [TestCase(1,"First item", false)]
        public void ShouldCreateCorrectTodoItem(int id, string title, bool done)
        {
            var todo = new TodoItem(id, title, done);

            todo.ListId.Should().Be(1);
            todo.Title.Should().Be("First item");
            todo.Done.Should().Be(false);
        }

        [Test]
        public void ShouldChangePriorityCorrectly()
        {
            var todoItem = new TodoItem(1, "Let's go");

            todoItem.Priority.Should().Be(PriorityLevel.None);

            todoItem.ChangePriority(PriorityLevel.High);

            todoItem.Priority.Should().Be(PriorityLevel.High);
        }
    }
}