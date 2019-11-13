using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.WebUI.IntegrationTests.Controllers.TodoItems
{
    public class Update : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public Update(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GivenValidUpdateTodoItemCommand_ReturnsSuccessCode()
        {
            var client = await _factory.GetAuthenticatedClientAsync();

            var command = new UpdateTodoItemCommand
            { 
                Id = 1, 
                Title = "Do this thing.",
                Done = true
            };

            var content = IntegrationTestHelper.GetRequestContent(command);

            var response = await client.PutAsync($"/api/todoitems/{command.Id}", content);

            response.EnsureSuccessStatusCode();
        }
    }
}
