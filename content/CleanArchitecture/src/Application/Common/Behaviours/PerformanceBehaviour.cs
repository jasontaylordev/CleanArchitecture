using Cubido.Template.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Cubido.Template.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IMessage
{
    private readonly Stopwatch timer;
    private readonly ILogger<TRequest> logger;
    private readonly IUser user;
    private readonly IIdentityService identityService;

    public PerformanceBehaviour(
        ILogger<TRequest> logger,
        IUser user,
        IIdentityService identityService)
    {
        timer = new Stopwatch();

        this.logger = logger;
        this.user = user;
        this.identityService = identityService;
    }

    public async ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        timer.Start();

        var response = await next(request, cancellationToken);

        timer.Stop();

        var elapsedMilliseconds = timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = user.Id ?? string.Empty;
            var userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await identityService.GetUserNameAsync(userId);
            }

            logger.LogWarning("Cubido.Template Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userId, userName, request);
        }

        return response;
    }
}
