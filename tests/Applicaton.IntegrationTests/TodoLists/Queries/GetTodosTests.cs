using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Queries
{
    using static Testing;

    public class GetTodosTests : TestBase
    {
        [Test]
        public async Task ShouldIncludePriorityLevels()
        {
            var query = new GetTodosQuery();

            var result = await SendAsync(query);

            result.PriorityLevels.Should().NotBeEmpty();
        }

        [Test]
        public async Task ShouldGetAllListsAndItems()
        {
            var query = new GetTodosQuery();

            var result = await SendAsync(query);

            result.Lists.Should().HaveCount(0);
        }
    }
}
