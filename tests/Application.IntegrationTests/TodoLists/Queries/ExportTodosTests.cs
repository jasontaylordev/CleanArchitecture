using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Queries
{
    using static Testing;

    public class ExportTodosTests : TestBase
    {
        [Test]
        public void ShouldDenyAnonymousUser()
        {
            var query = new ExportTodosQuery();

            query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<UnauthorizedAccessException>();
        }

        [Test]
        public async Task ShouldDenyNonAdministrator()
        {
            await RunAsDefaultUserAsync();

            var query = new ExportTodosQuery();

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<ForbiddenAccessException>();
        }

        [Test]
        public async Task ShouldAllowAdministrator()
        {
            await RunAsAdministratorAsync();

            var listId = await SendAsync(new CreateTodoListCommand
            {
                Title = "New List"
            });

            var query = new ExportTodosQuery
            {
                ListId = listId
            };

            FluentActions.Invoking(() => SendAsync(query))
                .Should().NotThrow<ForbiddenAccessException>();
        }
    }
}
