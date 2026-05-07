using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Email;

public class DevelopmentEmailService : IEmailService
{
    private readonly ApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ILogger<DevelopmentEmailService> _logger;

    public DevelopmentEmailService(ApplicationDbContext context, IIdentityService identityService, ILogger<DevelopmentEmailService> logger)
    {
        _context = context;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task SendAsync(string toUserId, string subject, string body, CancellationToken cancellationToken)
    {
        var to = await _identityService.GetUserNameAsync(toUserId) ?? toUserId;

        _context.EmailOutboxMessages.Add(new EmailOutboxMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            Status = "Queued"
        });

        _logger.LogInformation("Development email queued. To: {To}; Subject: {Subject}; Body: {Body}", to, subject, body);
    }
}
