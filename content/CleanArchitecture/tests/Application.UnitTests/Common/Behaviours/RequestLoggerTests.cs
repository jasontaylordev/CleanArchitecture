using Cubido.Template.Application.Common.Behaviours;
using Cubido.Template.Application.Common.Interfaces;
using Cubido.Template.Application.TodoItems.Commands.CreateTodoItem;
using Microsoft.Extensions.Logging;

namespace Cubido.Template.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private ILogger<CreateTodoItemCommand> mockLogger = null!;
    private IUser mockUser = null!;
    private IIdentityService mockIdentityService = null!;

    [SetUp]
    public void Setup()
    {
        mockLogger = Substitute.For<ILogger<CreateTodoItemCommand>>();
        mockUser = Substitute.For<IUser>();
        mockIdentityService = Substitute.For<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        mockUser.Id.Returns(_ => Guid.NewGuid().ToString());

        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand, ValueTask>(mockLogger, mockUser, mockIdentityService);

        await requestLogger.Handle(new CreateTodoItemCommand { ListId = 1, Title = "title" }, (_, _) => default, new CancellationToken());
        _ = mockIdentityService.Received(1).GetUserNameAsync(Arg.Any<string>());
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand, ValueTask>(mockLogger, mockUser, mockIdentityService);

        await requestLogger.Handle(new CreateTodoItemCommand { ListId = 1, Title = "title" }, (_, _) => default, new CancellationToken());

        _ = mockIdentityService.DidNotReceive().GetUserNameAsync(Arg.Any<string>());
    }
}
