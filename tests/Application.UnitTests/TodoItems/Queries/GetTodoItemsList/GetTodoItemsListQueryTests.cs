using AutoMapper;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsList;
using CleanArchitecture.Application.UnitTests.Common;
using CleanArchitecture.Infrastructure.Persistence;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.TodoItems.Queries.GetTodoItemsList
{
    [Collection("QueryCollection")]
    public class GetTodoItemsListQueryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetTodoItemsListQueryTests(QueryTestFixture fixture)
        {
            _context = fixture.Context;
            _mapper = fixture.Mapper;
        }

        [Fact]
        public async Task Handle_ReturnsCorrectVmAndTodoItemsCount()
        {
            var sut = new GetTodoItemsListQuery.GetTodoItemsListQueryHandler(_context, _mapper);

            var result = await sut.Handle(new GetTodoItemsListQuery(), CancellationToken.None);

            result.ShouldBeOfType<TodoItemsListVm>();
            result.TodoItems.Count.ShouldBe(4);
        }
    }
}
