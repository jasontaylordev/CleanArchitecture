using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;

namespace CleanArchitecture.Application.UnitTests.Common
{
    public static class ApplicationDbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var operationalStoreOptions = Options.Create(
                new OperationalStoreOptions
                {
                    DeviceFlowCodes = new TableConfiguration("DeviceCodes"),
                    PersistedGrants = new TableConfiguration("PersistedGrants")
                });

            var dateTimeMock = new Mock<IDateTime>();
            dateTimeMock.Setup(m => m.Now)
                .Returns(new DateTime(3001, 1, 1));

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(m => m.UserId)
                .Returns("00000000-0000-0000-0000-000000000000");

            var context = new ApplicationDbContext(
                options, operationalStoreOptions,
                currentUserServiceMock.Object, dateTimeMock.Object);

            context.Database.EnsureCreated();

            SeedSampleData(context);

            return context;
        }

        public static void SeedSampleData(ApplicationDbContext context)
        {
            context.TodoLists.AddRange(
                new TodoList { Id = 1, Title = "Shopping" }
            );

            context.TodoItems.AddRange(
                new TodoItem { Id = 1, ListId = 1, Title = "Bread", Done = true },
                new TodoItem { Id = 2, ListId = 1, Title = "Butter", Done = true },
                new TodoItem { Id = 3, ListId = 1, Title = "Milk" },
                new TodoItem { Id = 4, ListId = 1, Title = "Sugar" },
                new TodoItem { Id = 5, ListId = 1, Title = "Coffee" }
            );

            context.SaveChanges();
        }

        public static void Destroy(ApplicationDbContext context)
        {
            context.Database.EnsureDeleted();

            context.Dispose();
        }
    }
}