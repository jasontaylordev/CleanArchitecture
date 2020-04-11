using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.NUnitTests.TodoLists.Com
{
    using static Testing;

    public class GetTodosTests : TestBase
    {
        [Test]
        public async Task ShouldGetAllListsAndItems()
        {
            var query = new GetTodosQuery();

            var result = await SendAsync(query);

            result.Lists.Should().HaveCount(0);
        }
    }
}
