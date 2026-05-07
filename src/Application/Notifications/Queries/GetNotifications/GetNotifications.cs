using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Notifications.Queries.GetNotifications;

public record NotificationDto
{
    public int Id { get; init; }

    public string UserId { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public string LinkUrl { get; init; } = string.Empty;

    public bool IsRead { get; init; }

    public DateTimeOffset Created { get; init; }
}

public record GetNotificationsQuery : IRequest<IReadOnlyCollection<NotificationDto>>;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IReadOnlyCollection<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetNotificationsQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<IReadOnlyCollection<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_user.Id))
        {
            throw new ForbiddenAccessException();
        }

        return await _context.UserNotifications
            .AsNoTracking()
            .Where(n => n.UserId == _user.Id)
            .OrderByDescending(n => n.Id)
            .Take(25)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                LinkUrl = n.LinkUrl,
                IsRead = n.IsRead,
                Created = n.Created
            })
            .ToListAsync(cancellationToken);
    }
}
