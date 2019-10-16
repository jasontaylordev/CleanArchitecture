using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.UnitTests.Common;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.TodoItems.Commands.CreateTodoItem
{
    public class CreateTodoItemCommandTests : CommandTestBase
    {
        [Fact]
        public async Task Handle_ShouldPersistTodoItem()
        {
            var command = new CreateTodoItemCommand
            {
                Name = "Do yet another thing."
            };

            var sut = new CreateTodoItemCommand.CreateTodoItemCommandHandler(Context);

            var result = await sut.Handle(command, CancellationToken.None);

            var entity = Context.TodoItems.Find(result);

            entity.ShouldNotBeNull();
            entity.Name.ShouldBe(command.Name);
            entity.IsComplete.ShouldBeFalse();
        }
    }
}
