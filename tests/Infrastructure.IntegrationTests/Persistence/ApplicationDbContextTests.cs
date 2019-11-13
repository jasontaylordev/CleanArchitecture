using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Infrastructure.IntegrationTests.Persistence
{
    public class ApplicationDbContextTests : IDisposable
    {
        private readonly string _userId;
        private readonly DateTime _dateTime;
        private readonly Mock<IDateTime> _dateTimeMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly ApplicationDbContext _sut;

        public ApplicationDbContextTests()
        {
            _dateTime = new DateTime(3001, 1, 1);
            _dateTimeMock = new Mock<IDateTime>();
            _dateTimeMock.Setup(m => m.Now).Returns(_dateTime);

            _userId = "00000000-0000-0000-0000-000000000000";
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(m => m.UserId).Returns(_userId);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var operationalStoreOptions = Options.Create(
                new OperationalStoreOptions
                {
                    DeviceFlowCodes = new TableConfiguration("DeviceCodes"),
                    PersistedGrants = new TableConfiguration("PersistedGrants")
                });

            _sut = new ApplicationDbContext(options, operationalStoreOptions, _currentUserServiceMock.Object, _dateTimeMock.Object);

            _sut.TodoItems.Add(new TodoItem
            {
                Id = 1,
                Title = "Do this thing."
            });

            _sut.SaveChanges();
        }

        [Fact]
        public async Task SaveChangesAsync_GivenNewTodoItem_ShouldSetCreatedProperties()
        {
            var item = new TodoItem
            {
                Id = 2,
                Title = "This thing is done.",
                Done = true
            };

            _sut.TodoItems.Add(item);

            await _sut.SaveChangesAsync();

            item.Created.ShouldBe(_dateTime);
            item.CreatedBy.ShouldBe(_userId);
        }

        [Fact]
        public async Task SaveChangesAsync_GivenExistingTodoItem_ShouldSetLastModifiedProperties()
        {
            long id = 1;

            var item = await _sut.TodoItems.FindAsync(id);

            item.Done = true;

            await _sut.SaveChangesAsync();

            item.LastModified.ShouldNotBeNull();
            item.LastModified.ShouldBe(_dateTime);
            item.LastModifiedBy.ShouldBe(_userId);
        }

        public void Dispose()
        {
            _sut?.Dispose();
        }
    }
}
