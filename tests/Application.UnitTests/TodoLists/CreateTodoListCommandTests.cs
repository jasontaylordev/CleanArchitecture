using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using CleanArchitecture.Application.UnitTests._Data;
using CleanArchitecture.Application.UnitTests.Infrastructure;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.TodoLists
{
    public class CreateTodoListCommandTests : TestBase
    {
        private readonly Mock<IApplicationDbContext> _dbMock = new Mock<IApplicationDbContext>();
        private Mock<DbSet<TodoList>> _todoListDbSetMock = new Mock<DbSet<TodoList>>();

        private CreateTodoListCommandValidator _createCommandValidator;
        private IRequestHandler<CreateTodoListCommand, int> _handler;

        public override void Init()
        {
            base.Init();

            MockData();

            _createCommandValidator = new CreateTodoListCommandValidator(_dbMock.Object);

            _handler = new CreateTodoListCommandHandler(_dbMock.Object);
        }

        private void MockData()
        {
            _todoListDbSetMock = _dbMock.SetDbSetData(ctx => ctx.TodoLists, TodoTestData.TodoListData);
        }

        [Test]
        public void ShouldNotAllowEmptyTitle()
        {
            var validationResults = _createCommandValidator.Validate(new CreateTodoListCommand());
            validationResults.Errors.Count.Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void ShouldNotAllowVeryLongTitles()
        {
            string veryLongTitle = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.\r\n\r\nWhy do we use it?\r\nIt is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout. The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here', making it look like readable English. Many desktop publishing packages and web page editors now use Lorem Ipsum as their default model text, and a search for 'lorem ipsum' will uncover many web sites still in their infancy. Various versions have evolved over the years, sometimes by accident, sometimes on purpose (injected humour and the like).";
    
            var validationResults = _createCommandValidator.Validate(
                new CreateTodoListCommand { Title = veryLongTitle });
            validationResults.Errors.Count.Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void ShouldNotAllowDuplicateTitle()
        {
            string existingTitle = TodoTestData.TodoListData.First().Title;
            var validationResults = _createCommandValidator.Validate(
                new CreateTodoListCommand { Title = existingTitle });
            validationResults.Errors.Count.Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public async Task ShouldUseTheProvidedTitle()
        {
            string theTitle = "some title";
            await _handler.Handle(new CreateTodoListCommand {Title = theTitle}, default);

            _todoListDbSetMock.Verify(m => m.Add(It.Is<TodoList>(td => td.Title == theTitle)));
        }

    }
}
