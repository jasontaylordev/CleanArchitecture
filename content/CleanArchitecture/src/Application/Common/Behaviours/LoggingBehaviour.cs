using Cubido.Template.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cubido.Template.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse> : MessagePreProcessor<TRequest, TResponse> where TRequest : IMessage
{
    private readonly ILogger logger;
    private readonly IUser user;
    private readonly IIdentityService identityService;

    public LoggingBehaviour(ILogger<TRequest> logger, IUser user, IIdentityService identityService)
    {
        this.logger = logger;
        this.user = user;
        this.identityService = identityService;
    }

    protected override async ValueTask Handle(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = user.Id ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await identityService.GetUserNameAsync(userId);
        }

        logger.LogInformation("Cubido.Template Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);
    }
}
