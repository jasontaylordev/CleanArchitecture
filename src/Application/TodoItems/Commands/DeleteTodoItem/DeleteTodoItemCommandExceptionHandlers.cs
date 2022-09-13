using CleanArchitecture.Application.Common.Exceptions;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;

public class DeleteTodoItemNotFoundExceptionHandler : IRequestExceptionHandler<DeleteTodoItemCommand, Unit, NotFoundException>
{
    private readonly ILogger _logger;
    
    public DeleteTodoItemNotFoundExceptionHandler(ILogger<DeleteTodoItemNotFoundExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(DeleteTodoItemCommand request,
        NotFoundException exception,
        RequestExceptionHandlerState<Unit> state,
        CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        
        _logger.LogWarning(exception,
            "CleanArchitecture Request: Handled Exception for Request {Name} {@Request}",
            requestName, request);
        
        state.SetHandled(default);
        
        return Task.CompletedTask;
    }
}