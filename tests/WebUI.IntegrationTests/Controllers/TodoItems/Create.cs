using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using Shouldly;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.WebUI.IntegrationTests.Controllers.TodoItems
{
    public class Create : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public Create(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GivenValidCreateTodoItemCommand_ReturnsSuccessCode()
        {
            var client = await _factory.GetAuthenticatedClientAsync();

            var command = new CreateTodoItemCommand
            {
                Title = "Do yet another thing."
            };

            var content = IntegrationTestHelper.GetRequestContent(command);

            var response = await client.PostAsync($"/api/todoitems", content);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GivenInvalidCreateTodoItemCommand_ReturnsBadRequest()
        {
            var client = await _factory.GetAuthenticatedClientAsync();

            var command = new CreateTodoItemCommand
            {
                Title = "This description of this thing will exceed the maximum length. This description of this thing will exceed the maximum length. This description of this thing will exceed the maximum length. This description of this thing will exceed the maximum length."
            };

            var content = IntegrationTestHelper.GetRequestContent(command);

            var response = await client.PostAsync($"/api/todoitems", content);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}
