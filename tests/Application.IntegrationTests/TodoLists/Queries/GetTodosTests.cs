using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;
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
            await RunAsDefaultUserAsync();

            var query = new GetTodosQuery();

            var result = await SendAsync(query);

            result.PriorityLevels.Should().NotBeEmpty();
        }

        [Test]
        public async Task ShouldReturnAllListsAndItems()
        {
            await RunAsDefaultUserAsync();

            await AddAsync(new TodoList
            {
                Title = "Shopping",
                Items =
                    {
                        new TodoItem { Title = "Apples", Done = true },
                        new TodoItem { Title = "Milk", Done = true },
                        new TodoItem { Title = "Bread", Done = true },
                        new TodoItem { Title = "Toilet paper" },
                        new TodoItem { Title = "Pasta" },
                        new TodoItem { Title = "Tissues" },
                        new TodoItem { Title = "Tuna" }
                    }
            });

            var query = new GetTodosQuery();

            var result = await SendAsync(query);

            result.Lists.Should().HaveCount(1);
            result.Lists.First().Items.Should().HaveCount(7);
        }

        [Test]
        public void ShouldRequireAuthorization()
        {
            var query = new GetTodosQuery();

            query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<UnauthorizedAccessException>();
        }
    }
}
