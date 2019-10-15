using CleanArchitecture.Application.TodoItems.Queries.GetTodoItem;
using Shouldly;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.WebUI.IntegrationTests.Controllers.TodoItems
{
    public class Get : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public Get(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GivenValidId_ReturnsTodoItemVm()
        {
            var client = await _factory.GetAuthenticatedClientAsync();
            var validId = 1;

            var response = await client.GetAsync($"/api/todoitems/{validId}");

            response.EnsureSuccessStatusCode();

            var vm = await IntegrationTestHelper.GetResponseContent<TodoItemVm>(response);

            vm.ShouldBeOfType<TodoItemVm>();
            vm.Id.ShouldBe(1);
        }

        [Fact]
        public async Task GivenInvalidId_ReturnsNotFound()
        {
            var client = await _factory.GetAuthenticatedClientAsync();
            var invalidId = 99;

            var response = await client.GetAsync($"/api/todoitems/{invalidId}");

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}
