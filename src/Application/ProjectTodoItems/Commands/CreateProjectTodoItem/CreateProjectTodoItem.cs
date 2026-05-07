using System.Globalization;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Notifications.Queries.GetNotifications;
using CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectTodoItems;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.ProjectTodoItems.Commands.CreateProjectTodoItem;

public record CreateProjectTodoItemCommand : IRequest<int>
{
    public int ProjectId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? DueDate { get; init; }

    public string? AssigneeUserId { get; init; }

    public int? StatusId { get; init; }
}

public class CreateProjectTodoItemCommandValidator : AbstractValidator<CreateProjectTodoItemCommand>
{
    public CreateProjectTodoItemCommandValidator()
    {
        RuleFor(v => v.ProjectId).GreaterThan(0);
        RuleFor(v => v.Title).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Description).MaximumLength(2000);
        RuleFor(v => v.DueDate)
            .Must(BeValidDateOnly)
            .When(v => !string.IsNullOrWhiteSpace(v.DueDate))
            .WithMessage("Due date must use yyyy-MM-dd format.");
    }

    private static bool BeValidDateOnly(string? value)
    {
        return DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
}

public class CreateProjectTodoItemCommandHandler : IRequestHandler<CreateProjectTodoItemCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;
    private readonly IProjectTodoRealtimeNotifier _notifier;
    private readonly IEmailService _emailService;

    public CreateProjectTodoItemCommandHandler(IApplicationDbContext context, IUser user, IIdentityService identityService, IProjectTodoRealtimeNotifier notifier, IEmailService emailService)
    {
        _context = context;
        _user = user;
        _identityService = identityService;
        _notifier = notifier;
        _emailService = emailService;
    }

    public async Task<int> Handle(CreateProjectTodoItemCommand request, CancellationToken cancellationToken)
    {
        var reporterUserId = _user.Id;

        if (string.IsNullOrWhiteSpace(reporterUserId))
        {
            throw new ForbiddenAccessException();
        }

        var projectExists = await _context.Projects.AnyAsync(p => p.Id == request.ProjectId, cancellationToken);
        Guard.Against.NotFound(request.ProjectId, projectExists ? request.ProjectId : (int?)null);

        var statusId = request.StatusId ?? await _context.ProjectTodoStatuses
            .Where(s => s.ProjectId == request.ProjectId && s.IsDefault)
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (statusId == 0)
        {
            statusId = await _context.ProjectTodoStatuses
                .Where(s => s.ProjectId == request.ProjectId)
                .OrderBy(s => s.SortOrder)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        Guard.Against.NotFound(request.ProjectId, statusId == 0 ? (int?)null : request.ProjectId);

        var statusIsValid = await _context.ProjectTodoStatuses.AnyAsync(s => s.Id == statusId && s.ProjectId == request.ProjectId, cancellationToken);
        Guard.Against.NotFound(statusId, statusIsValid ? statusId : (int?)null);

        var item = new ProjectTodoItem
        {
            ProjectId = request.ProjectId,
            Title = request.Title,
            Description = request.Description,
            DueDate = ParseDateOnly(request.DueDate),
            AssigneeUserId = request.AssigneeUserId,
            ReporterUserId = reporterUserId,
            StatusId = statusId
        };

        _context.ProjectTodoItems.Add(item);

        await _context.SaveChangesAsync(cancellationToken);

        UserNotification? notification = null;

        if (!string.IsNullOrWhiteSpace(request.AssigneeUserId))
        {
            notification = new UserNotification
            {
                UserId = request.AssigneeUserId,
                Title = "New project to-do assignment",
                Message = $"You have been assigned to '{request.Title}'.",
                LinkUrl = $"/projects/{request.ProjectId}/todos/{item.Id}"
            };

            _context.UserNotifications.Add(notification);

            await _emailService.SendAsync(
                request.AssigneeUserId,
                $"Project to-do assigned: {request.Title}",
                $"You have been assigned to '{request.Title}'. Open /projects/{request.ProjectId}/todos/{item.Id} to review it.",
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        var savedItem = await _context.ProjectTodoItems
            .AsNoTracking()
            .Include(i => i.Status)
            .FirstAsync(i => i.Id == item.Id, cancellationToken);

        await _notifier.ProjectTodoItemCreatedAsync(await ProjectTodoItemDtoFactory.CreateAsync(savedItem, _identityService), cancellationToken);

        if (notification is not null)
        {
            await _notifier.UserNotificationCreatedAsync(new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                LinkUrl = notification.LinkUrl,
                IsRead = notification.IsRead,
                Created = notification.Created
            }, cancellationToken);
        }

        return item.Id;
    }

    private static DateOnly? ParseDateOnly(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : DateOnly.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
