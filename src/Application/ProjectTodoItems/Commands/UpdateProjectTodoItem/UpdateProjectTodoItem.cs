using System.Globalization;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectTodoItems;

namespace CleanArchitecture.Application.ProjectTodoItems.Commands.UpdateProjectTodoItem;

public record UpdateProjectTodoItemCommand : IRequest
{
    public int Id { get; init; }

    public int ProjectId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? DueDate { get; init; }
}

public class UpdateProjectTodoItemCommandValidator : AbstractValidator<UpdateProjectTodoItemCommand>
{
    public UpdateProjectTodoItemCommandValidator()
    {
        RuleFor(v => v.Id).GreaterThan(0);
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

public class UpdateProjectTodoItemCommandHandler : IRequestHandler<UpdateProjectTodoItemCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IProjectTodoRealtimeNotifier _notifier;

    public UpdateProjectTodoItemCommandHandler(IApplicationDbContext context, IIdentityService identityService, IProjectTodoRealtimeNotifier notifier)
    {
        _context = context;
        _identityService = identityService;
        _notifier = notifier;
    }

    public async Task Handle(UpdateProjectTodoItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ProjectTodoItems
            .Include(i => i.Status)
            .FirstOrDefaultAsync(i => i.Id == request.Id && i.ProjectId == request.ProjectId, cancellationToken);

        Guard.Against.NotFound(request.Id, item);

        item.Title = request.Title;
        item.Description = request.Description;
        item.DueDate = ParseDateOnly(request.DueDate);

        await _context.SaveChangesAsync(cancellationToken);

        await _notifier.ProjectTodoItemUpdatedAsync(await ProjectTodoItemDtoFactory.CreateAsync(item, _identityService), cancellationToken);
    }

    private static DateOnly? ParseDateOnly(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : DateOnly.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
