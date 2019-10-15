using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsList;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace CleanArchitecture.WebUI.IntegrationTests.Controllers.TodoItems
{
    public class GetAll : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public GetAll(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ReturnsTodoItemsListVm()
        {
            var client = await _factory.GetAuthenticatedClientAsync();

            var response = await client.GetAsync("/api/todoitems");

            response.EnsureSuccessStatusCode();

            var vm = await IntegrationTestHelper.GetResponseContent<TodoItemsListVm>(response);

            vm.ShouldBeOfType<TodoItemsListVm>();
            vm.TodoItems.Count.ShouldBe(4);
        }
    }
}
