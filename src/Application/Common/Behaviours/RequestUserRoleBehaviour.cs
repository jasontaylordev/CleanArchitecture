using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class RequestUserRoleBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public RequestUserRoleBehaviour(ILogger<TRequest> logger,
            ICurrentUserService currentUserService,
            IIdentityService identityService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _identityService = identityService;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!(request is IRequestRequiresUserRole<TResponse>))
            {
                return await next();
            }

            var userRoleRequest = request as IRequestRequiresUserRole<TResponse>;
            var userIsInRoleResult = await _identityService.UserIsInRoleAsync(_currentUserService.UserId, userRoleRequest.RequiredRoles);

            if (!userIsInRoleResult.Succeeded)
            {
                _logger.LogWarning($"User: {_currentUserService.UserId} is not in role: {string.Join(", ", userRoleRequest.RequiredRoles)}.");
                throw new UnauthorizedAccessException(string.Join(". ", userIsInRoleResult.Errors));
            }

            return await next();
        }
    }
}
