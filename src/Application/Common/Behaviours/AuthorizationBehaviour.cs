using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public AuthorizationBehaviour(
            ILogger<TRequest> logger,
            ICurrentUserService currentUserService,
            IIdentityService identityService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

            if (!authorizeAttributes.Any())
            {
                // Must be authenticated user
                if (_currentUserService.UserId == null)
                {
                    throw new UnauthorizedAccessException();
                }

                var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

                if (authorizeAttributesWithRoles.Any())
                {
                    foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                    {
                        var authorized = false;
                        foreach (var role in roles)
                        {
                            var isInRole = await _identityService.UserIsInRole(_currentUserService.UserId, role.Trim());
                            if (isInRole)
                            {
                                authorized = true;
                                continue;
                            }
                        }

                        // Must be a member of at least one role in roles
                        if (!authorized)
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }
                }
            }

            // User is authorized / authorization not required
            return await next();
        }
    }
}
