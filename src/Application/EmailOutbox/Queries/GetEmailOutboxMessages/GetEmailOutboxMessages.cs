using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.EmailOutbox.Queries.GetEmailOutboxMessages;

public record EmailOutboxMessageDto
{
    public int Id { get; init; }

    public string To { get; init; } = string.Empty;

    public string Subject { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public DateTimeOffset? SentAt { get; init; }

    public DateTimeOffset Created { get; init; }
}

public record GetEmailOutboxMessagesQuery : IRequest<IReadOnlyCollection<EmailOutboxMessageDto>>;

public class GetEmailOutboxMessagesQueryHandler : IRequestHandler<GetEmailOutboxMessagesQuery, IReadOnlyCollection<EmailOutboxMessageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEmailOutboxMessagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<EmailOutboxMessageDto>> Handle(GetEmailOutboxMessagesQuery request, CancellationToken cancellationToken)
    {
        return await _context.EmailOutboxMessages
            .AsNoTracking()
            .OrderByDescending(m => m.Id)
            .Take(25)
            .Select(m => new EmailOutboxMessageDto
            {
                Id = m.Id,
                To = m.To,
                Subject = m.Subject,
                Body = m.Body,
                Status = m.Status,
                SentAt = m.SentAt,
                Created = m.Created
            })
            .ToListAsync(cancellationToken);
    }
}
