using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Notifications.Commands.MarkNotificationRead;

public record MarkNotificationReadCommand(int Id) : IRequest;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public MarkNotificationReadCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_user.Id))
        {
            throw new ForbiddenAccessException();
        }

        var notification = await _context.UserNotifications
            .FirstOrDefaultAsync(n => n.Id == request.Id && n.UserId == _user.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, notification);

        notification.IsRead = true;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
