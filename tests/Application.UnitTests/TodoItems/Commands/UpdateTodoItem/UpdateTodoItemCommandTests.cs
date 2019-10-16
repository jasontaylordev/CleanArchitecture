using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture.Application.UnitTests.Common;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.TodoItems.Commands.UpdateTodoItem
{
    public class UpdateTodoItemCommandTests : CommandTestBase
    {
        [Fact]
        public async Task Handle_GivenValidId_ShouldUpdatePersistedTodoItem()
        {
            var command = new UpdateTodoItemCommand
            {
                Id = 1,
                Name = "This thing is also done.",
                IsComplete = true
            };

            var sut = new UpdateTodoItemCommand.UpdateTodoItemCommandHandler(Context);

            await sut.Handle(command, CancellationToken.None);

            var entity = Context.TodoItems.Find(command.Id);

            entity.ShouldNotBeNull();
            entity.Name.ShouldBe(command.Name);
            entity.IsComplete.ShouldBeTrue();
        }

        [Fact]
        public void Handle_GivenInvalidId_ThrowsException()
        {
            var command = new UpdateTodoItemCommand
            {
                Id = 99,
                Name = "This item doesn't exist.",
                IsComplete = false
            };

            var sut = new UpdateTodoItemCommand.UpdateTodoItemCommandHandler(Context);

            Should.ThrowAsync<NotFoundException>(() => 
                sut.Handle(command, CancellationToken.None));
        }
    }
}