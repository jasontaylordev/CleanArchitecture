using System;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using CleanArchitecture.Application.UnitTests._Data;
using CleanArchitecture.Application.UnitTests.Infrastructure;
using CleanArchitecture.Domain.Enums;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.TodoLists
{
    public class GetTodosQueryTests : TestBase
    {
        private readonly Mock<IApplicationDbContext> _dbMock = new Mock<IApplicationDbContext>();
        private IRequestHandler<GetTodosQuery, TodosVm> _handler;

        public override void Init()
        {
            base.Init();

            MockData();
            _handler = new GetTodosQueryHandler(_dbMock.Object, Mapper);
        }

        private void MockData()
        {
            _dbMock.SetDbSetData(ctx => ctx.TodoLists, TodoTestData.TodoListData);
        }

        [Test]
        public async Task ShouldReturnTheExistingListCount()
        {
            var items = await _handler.Handle(new GetTodosQuery(), default);
            items.Lists.Count.Should().Be(TodoTestData.TodoListData.Count);
        }

        [Test]
        public async Task ShouldGetAllPriorityLevels()
        {
            var items = await _handler.Handle(new GetTodosQuery(), default);
            items.PriorityLevels.Count.Should().Be(Enum.GetValues(typeof(PriorityLevel)).Length);
        }
    }
}
