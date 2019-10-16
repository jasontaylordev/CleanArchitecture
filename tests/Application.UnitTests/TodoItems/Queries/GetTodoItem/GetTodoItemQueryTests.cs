using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItem;
using CleanArchitecture.Application.UnitTests.Common;
using CleanArchitecture.Infrastructure.Persistence;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.TodoItems.Queries.GetTodoItem
{
    [Collection("QueryCollection")]
    public class GetTodoItemQueryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetTodoItemQueryTests(QueryTestFixture fixture)
        {
            _context = fixture.Context;
            _mapper = fixture.Mapper;
        }

        [Fact]
        public async Task Handle_GivenValidId_ReturnsCorrectVm()
        {
            var query = new GetTodoItemQuery
            {
                Id = 1
            };

            var sut = new GetTodoItemQuery.GetTodoItemQueryHandler(_context, _mapper);

            var result = await sut.Handle(query, CancellationToken.None);

            result.ShouldBeOfType<TodoItemVm>();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public void Handle_GivenInvalidId_ThrowsException()
        {
            var query = new GetTodoItemQuery
            {
                Id = 99
            };

            var sut = new GetTodoItemQuery.GetTodoItemQueryHandler(_context, _mapper);

            Should.ThrowAsync<NotFoundException>(() => 
                sut.Handle(query, CancellationToken.None));
        }
    }
}
