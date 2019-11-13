using AutoMapper;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using CleanArchitecture.Application.UnitTests.Common;
using CleanArchitecture.Infrastructure.Persistence;
using Shouldly;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.TodoLists.Queries.GetTodos
{
    [Collection("QueryTests")]
    public class GetTodosQueryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetTodosQueryTests(QueryTestFixture fixture)
        {
            _context = fixture.Context;
            _mapper = fixture.Mapper;
        }

        [Fact]
        public async Task Handle_ReturnsCorrectVmAndListCount()
        {
            var query = new GetTodosQuery();

            var handler = new GetTodosQuery.GetTodosQueryHandler(_context, _mapper);

            var result = await handler.Handle(query, CancellationToken.None);

            result.ShouldBeOfType<TodosVm>();
            result.Lists.Count.ShouldBe(1);

            var list = result.Lists.First();

            list.Items.Count.ShouldBe(5);
        }
    }
}
