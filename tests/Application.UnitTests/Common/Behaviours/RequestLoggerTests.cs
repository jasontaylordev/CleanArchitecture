using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using Microsoft.Extensions.Logging;
using MitMediator;
using MitMediator.Tasks;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private Mock<ILogger<CreateTodoItemCommand>> _logger = null!;
    private Mock<IUser> _user = null!;
    private Mock<IIdentityService> _identityService = null!;

    class TestRequestHandlerNext: IRequestHandlerNext<CreateTodoItemCommand, int>
    {
        public ValueTask<int> InvokeAsync(CreateTodoItemCommand newRequest, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult(0);
        }
    }
    
    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<CreateTodoItemCommand>>();
        _user = new Mock<IUser>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _user.Setup(x => x.Id).Returns(Guid.NewGuid().ToString());

        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand, int>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.HandleAsync(new CreateTodoItemCommand { ListId = 1, Title = "title" }, new TestRequestHandlerNext(),  new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand, int>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.HandleAsync(new CreateTodoItemCommand { ListId = 1, Title = "title" }, new TestRequestHandlerNext(), new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Never);
    }
}
