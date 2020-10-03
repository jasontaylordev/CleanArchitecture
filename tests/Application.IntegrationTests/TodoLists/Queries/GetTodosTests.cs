using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Queries
{
    using static Testing;

    public class GetTodosTests : TestBase
    {
        [Test]
        public async Task ShouldReturnPriorityLevels()
        {
            var query = new GetTodosQuery();

            var result = await SendAsync(query);

            result.PriorityLevels.Should().NotBeEmpty();
        }

        [Test]
        public async Task ShouldReturnAllListsAndItems()
        {
            await AddAsync(new TodoList("Shopping")
            {
                Items =
                {
                    new TodoItem(1, "Apples", true),
                    new TodoItem(1, "Milk", true),
                    new TodoItem(1, "Bread", true),
                    new TodoItem(1, "Toilet paper", true),
                    new TodoItem(1, "Pasta", true),
                    new TodoItem(1, "Tissues", true),
                    new TodoItem(1, "Tuna", true),
                }
            });


            var query = new GetTodosQuery();

            var result = await SendAsync(query);

            result.Lists.Should().HaveCount(1);
            result.Lists.First().Items.Should().HaveCount(7);
        }
    }
}
