using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TException> : IRequestExceptionAction<TRequest, TException>
    where TRequest : notnull
    where TException : Exception
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public Task Execute(TRequest request, TException exception, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogError(exception, "CleanArchitecture Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);
        
        return Task.CompletedTask;
    }
}
